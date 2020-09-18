using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //a library used for sorting goals based on priority; goals require a set priority

public class SubGoal //helper divivde a goal into subgoals; subgoals are set to help make the system more flexible when having more complex objectives by leading an agent to the final goal (complete simulation for a single agent) through completing subgoals 
{
    public Dictionary<string, int> subgoals; //subgoals refer to the last effect of an action of a plan
    public bool remove; //to help determine when a goal must be removed from list of goals once it has been completed; this can also be used to repeat tasks (plans) by not setting the goal to be removed (example of a goal that is not removed from the system is the Nurse resting after being exhausted)

    public SubGoal(string s, int i, bool r) //a subgoal object has a string as the key identifier, and int as the value to determine its priority, and a boolean to determine if it should be removed or repeated all of which can be accessed when needed
    {
        //subgoals dictionary created with subgoal key,value pair
        subgoals = new Dictionary<string, int>();
        subgoals.Add(s, i);
        remove = r;
    }
}

public class Agent : MonoBehaviour //agent types are made as other scripts that inherit from the current one
{
    //Agents will have a list of actions available to them that can be perfomed as well as a list of goals to work towards
    public List<Action> actions = new List<Action>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>(); //agents keep track of goals they need to achieve

    public Inventory inventory = new Inventory(); //agents have access to available resources 
    public EnvironmentStates beliefs = new EnvironmentStates(); //agents have access to environment states (what the agent knows about the world)

    //GOAP based system planner which returns a queue of actions 
    Planner planner; //a way for the agent to access the planner
    Queue<Action> actionQueue; //to store the extracted actions from the planner
    public Action currentAction; //to determine the current action being performed by the agent instance 
    SubGoal currentGoal; //to determine the current goal being worked towards by the agent instance

    // Start is called before the first frame update
    public void Start() //start must be made public to allow child classes access 
    {
        //Array of actions -> versions of the Action class can be created as components and dropped onto the agent in the inspector. These need to be picked up when the agents start working and the actions will go into this created array
        Action[] acts = this.GetComponents<Action>(); //to retrieve the action components of the current agent instance
        foreach(Action a in acts) //for every action for this agent
        {
            actions.Add(a); //add it to the pool of actions that this agent can perform
        }
    }

    bool invoked = false; //boolean to determine if waiting has been invoked on an agent to show that the agent is performing an action

    void CompleteAction() //process the completion of an action
    {
        currentAction.running = false; //to force planning system to resume when notifying it that the agent finished running the action
        currentAction.PostPerform(); //action method call for action completion
        invoked = false; //reset invoked bool
    }

    void LateUpdate() //using late update to ensure everthing has been assigned before execution to avoid issues
    { //many coniditonals here are used to protect against a crash or error and help reset the agent when finding a plan fails. Resetting should allow the planner to formulate a new plan for the agent
        //if the agent is in the middle of performing an action, then that action should not be interrupted
        if (currentAction != null && currentAction.running)
        {
            float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position); //update distance the agent has to move to at the start of the loop in case it is not set later in LateUpdate
            if(currentAction.agent.hasPath && distanceToTarget < 2f) //if the agent has a goal and has reached that goal 
                //(using a set distance 1f from goal to signify that the agent is in the goal location which luckily makes a cluster of agents attempting to reach a location wait for the agents closest to the destination point to move before they can perform the action)
            {
                //run the action performing process shown by making the agent wait at the goal location for the specified duration based on the action
                if(!invoked) //boolean to determine if waiting process already invoked 
                {
                    Invoke("CompleteAction", currentAction.duration); //make the agent wait at the goal position reached to convey the action is being done (waiting time based on action performed)
                    invoked = true;
                }
            }
            return;
        }

        if(planner == null || actionQueue == null) //if neither a planner or action queue exists then the agent has nothing to work on and a must be assinged a plan 
        {
            planner = new Planner(); //create a planner

            //sort through subgoals from most important to least important to loop through them and find one for the agent to perfrom
            var sortedGoals = from entry in goals orderby entry.Value descending select entry; //use of Linq library to sort goals dictionary into sortedgoals 

            foreach(KeyValuePair<SubGoal, int> sg in sortedGoals) //loop through the ordered subgoals (starting from highest priority) that need to be satisfied 
            {
                actionQueue = planner.plan(actions, sg.Key.subgoals, beliefs); //call the planner to do planning process to give the agent the actions that it must perform (given null as the world state is being used at this point rather than the agent state)
                if(actionQueue != null) //if a plan has been found 
                {
                    currentGoal = sg.Key;//set the goal the agent is working towards as the agent's subgoal 
                    break;
                }
            }
        }

        //at this point the agent either runs out of actions to do (assuming it completed them) or it still has more actions it needs to perfrom
        if(actionQueue != null && actionQueue.Count == 0) //check queue exists to avoid errors and if the quque has no actions with it (empty queue)
        {
            if(currentGoal.remove) //if agent's current goal is removable, 
            {
                goals.Remove(currentGoal); //remove the goal from the list of goals the agent still needs to perfrom to avoid repeating it after completion (some goals need to be repeated based on goal which 
            }
            planner = null; //to trigger another planner being retrieved when done with current one for this iteration of the update
        }

        if(actionQueue != null && actionQueue.Count > 0) //ifthere are still actions in the queue that need to be performed (additional null check as safegaurd against crash)
        {
            currentAction = actionQueue.Dequeue(); //remove action at front of queue and store it in currentAction
            //perform checks required for the action
            if (currentAction.PrePerform()) 
            {
                //the targets for the navmesh agent to move to are set with tags referenced by strings (tags of objects are set from the inspector)
                if(currentAction.target == null && currentAction.targetTag != "") //check for the target object the agent has to move to. if thats not set, then set the one for the current action the agent is performing
                {
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }

                if(currentAction.target != null) //error check in case no agent has no position to go for perform action
                {
                    currentAction.running = true; //agent starts to perform action
                    currentAction.agent.SetDestination(currentAction.target.transform.position); //move agent to location needed to perform current action (Navmesh componenet will take care of navigating agents between its initial and goal positions)
                } 
            }
            else //in case PrePreform method cannot be excecuted 
            {
                actionQueue = null; //set the queueu to null to force the planner to get a new plan
            }
        }
    }
}
