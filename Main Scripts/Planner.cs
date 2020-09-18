using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node //a helper class used as an node which contains the action alognside its precondition and effect as well as capabilities to link it with other nodes to help create the action chains that make up the plan
{
    public Node parent; //to get the parent of this node (successor node)
    public float cost; //the cost of the action to help find cheapest plan
    public Dictionary<string, int> state; //the states of the environment for this node
    public Action action; //the action this node is referring to

    public Node(Node parent, float cost, Dictionary<string, int> allStates, Action action) // Constructor to setup all node variables
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allStates); //get a copy of the dictionary instead of modifying it 
        this.action = action;
    }

    public Node(Node parent, float cost, Dictionary<string, int> allStates, Dictionary<string, int> beliefStates, Action action) // overloading constructor to interject agent beliefs
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allStates); //populating environment states
        foreach(KeyValuePair<string, int> belief in beliefStates) //make sure both the enviornment states and agent beliefs are being fed through to the planner
        {
            if(!this.state.ContainsKey(belief.Key)) //if the belief doesn't exist (for that agent), then it should be added to the enviornment states
            {
                this.state.Add(belief.Key, belief.Value);
            }
        }
        this.action = action;
    }
}

public class Planner //planner class does not need to be monobehavior as it is  constructing the plan by finding the cheaptest set of actions without using Unity derived implemetation
{
    public Queue<Action> plan(List<Action> actions, Dictionary<string, int> goal, EnvironmentStates beliefstates) //method to return a plan for an agent to simulate given the actions an agent can perform, the goals that need to be satisfied by the agent type, and the state of agent 
    {
        List<Action> usableActions = new List<Action>(); //to store the list of actions that are usable/can be matched
        foreach (Action a in actions) //go through all actions
        {
            //discard actions that cannot be used
            if (a.IsAchievable()) //if an action can be performed
            {
                usableActions.Add(a); //then it is added to the usable list
            }
        }

        List<Node> leaves = new List<Node>(); //create a list to store the nodes
        Node start = new Node(null, 0.0f, Environment.Instance.GetWorld().GetStates(), beliefstates.GetStates(), null);  //the starting node from which the planner will construct the action chain for the agent to perform (given null parent and action with zero cost as it is the initial node)

        bool success = FindPlan(start, leaves, usableActions, goal); //return true or false based on if a succesful plan can be found (using called mehod) that leads to the goal

        if (!success) //when no plan can be found, give feedback through output log and return null
        {
            Debug.Log("CANNOT FORMULATE PLAN"); return null;
        }

        //if this point in the code is reached, that means a plan can be constructed to achieve the goal
        Node cheapest = null; //set cheapest node to null before finding it (in case not found or another issue occurs) The cheapest node is set as the initial node from which the planner will formulate the plan

        foreach (Node leaf in leaves) //loop through all nodes being used to contruct potential plans
        {
            if (cheapest == null) //if cheapest node has not been set
            {
                cheapest = leaf; //use the first node checked
            }
            else if (leaf.cost < cheapest.cost) //if the current node being checked is less than the cheapest node
            {
                cheapest = leaf; //then set the current node being checked as the cheapest one
            }
        }

        //when the cheapest node has been found, the planner will work backwards from it and across the chain to get a sequence of actions
        List<Action> result = new List<Action>(); //list to store actions that make up the resulting plan -> the action chain
        Node n = cheapest; //make a copy of the cheapest node to modify it without what node it was

        while (n != null) //while the cheapest node does exist 
        {
            if (n.action != null) //check that an action exists within this node (avoiding null error)
            {
                result.Insert(0, n.action); //record it in the plan
            }
            n = n.parent; //move onto the successor node (an so on until a plan is formulated)
        }

        Queue<Action> queue = new Queue<Action>(); //creating the queue of actions which agents can use to simulate the plan
        foreach (Action a in result) //extract the actions from the plan onto this queue 
        {
            queue.Enqueue(a); //the queue adds items starting from the end of the queue but since the planner worked bakcwards to formulate this plan, the there is not need to modify the order of the set of actions as a result
        }

        //debugging logs to observe the data during the simulation
        Debug.Log("PLAN FOUND");
        foreach (Action a in queue)
        {
            Debug.Log("Queue: " + a.actionName);
        }

        return queue; //method returns the queue of actions that makes up the plan
    }

    private bool FindPlan(Node parent, List<Node> leaves, List<Action> usableActions, Dictionary<string, int> goal) //recursive method that computes whether an plan can be made to achieve the goal 
    {
        bool foundPath = false; //using a bool as the condition to break recursion when a plan is possible

        foreach (Action action in usableActions) //for every usable action
        {
            if (action.IsAchievableGiven(parent.state)) //if that action can be perfromed given the state of the previous node (then this is the action the planner is looking for)
            {
                //currentState (var below) is going to be filled with the environment states. 
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state); //This dictionary is used to keep track of satisfied node matches including the conditions that are changing (by effects of actions) as the action chain is built
                foreach (KeyValuePair<string, int> eff in action.effects) //for every key value pair of action's effects
                {
                    if (!currentState.ContainsKey(eff.Key)) //if current state does not have the effect of the action in this
                    {
                        currentState.Add(eff.Key, eff.Value); //then record it in the current state so that it has the environment state and effect of the current action (when this funtion calls itself, the effect of the next action will be added and so on until the recursion breaks)
                    }
                }
                
                Node node = new Node(parent, parent.cost + action.cost, currentState, action); //update to the next node along the plan being constructed by the planner whilst tallying up the cost, passing along the accumulation of states and the current action

                if (GoalAchieved(goal, currentState)) //if the goal is achieved
                    {
                        //then the planner will match up the goal that its looking for with the current state (somewhere in current state if the goal is achieved)
                        leaves.Add(node);
                        foundPath = true;
                }
                else //otherwise if no plan is found to achieve the goal
                {
                    //then move onto to next node along the potential plan
                    List<Action> subset = ActionSubset(usableActions, action); //create a subset of usable actions that would decrease as this method in the planner continuously checks all possibilies for finding a potential working plan (should get smaller every time the FindPlan is called
                    bool found = FindPlan(node, leaves, subset, goal); //recursive call that checks the next possible node to create a working plan (the subset is given which discards already used actions)

                    if (found) //at some point in this recursion when a plan has been found that achieves the goal
                    {
                            foundPath = true; //foundPath is set to true to stop the recursion 
                    }
                }
            }
        }
        return foundPath; //returns the bool that determines if a plan was found that can achieve the goal
    }

    private List<Action> ActionSubset(List<Action> actions, Action toRemove) //method to remove the current action that's been assigned to the node (action already used) when finding a plan possible to achieve the goal through the FindPlan method
    {
        List<Action> subset = new List<Action>(); //create a list to store the used actions (for iterations when finding a working plan)

        foreach (Action a in actions) //for each action in the actions list
        {
            if (!a.Equals(toRemove)) //if the action that needs to be removed is not found
            {
                subset.Add(a); //then it is added to the subset list
            }
        }
        return subset; //the updated subset is returned
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state) //method to help determine if a goal is achieved when finding a plan possible to achieve the plan
    {
        foreach (KeyValuePair<string, int> g in goal) //for every key and value pair in the goal dictionary
        {
            if (!state.ContainsKey(g.Key)) //if the goal does not exists in the effects (meaning that it is not possible to reach as an effect of an action)
            {
                return false; //then the goal cannot be achieved
            }
        }
        //otherwise true is returned to signify that the goal is achieved for found plan 
        return true; 
    }
}