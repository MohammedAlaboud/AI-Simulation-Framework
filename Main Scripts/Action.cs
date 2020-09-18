using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //required for accessing AI functions

public abstract class Action : MonoBehaviour //using an abstract class to inherit from. This will act as a parent class for each action class made
{
    public string actionName = "Action"; //name of action given the default name of "Action"
    public float cost = 1.0f; //cost of the action, by default it is set to one -> The planner for this project will find the set of actions that achieve the goal with the least possible amount of cost accumulated by the actions 
    public GameObject target; //the location in which the action will or should take place, agents will be move to this locations to signify the action being performed
    public string targetTag; //the waypoint tags used to distinguish the significat locations within the simulation, and this is used to determine if the target exists
    public float duration = 0; //time it takes to perform the action (how long the agent will be in a specified location perfroming the action)
    
    //Accessible from the inspector to modify and add the precondition and effect for an action
    public EnvironmentState[] preConditions; //the condition that needs to be satisfied in order to perform the action
    public EnvironmentState[] afterEffects; //the resulting effect of performing the action

    public NavMeshAgent agent; //the NavMesh component attached to the agent for navigation

    //dictionaries used by the planner for matching precodnitions and effects to create required action chains, these are obtains from the preConditions and afterEffects variables which can be set from the inspector 
    public Dictionary<string, int> preconditions; //dictionary of action precodnitions 
    public Dictionary<string, int> effects; //dictionary of action effects

    public EnvironmentStates agentBeliefs; //internal state of the agent itself (the agents states are decoupled from the enviornment states; both are used to set preconditions and effects)

    public Inventory inventory; //used to point back to the agent that is performing the action to know the status of resources in the environment
    public EnvironmentStates beliefs; //to store the environment states the agent knows about (access from agent class)

    public bool running = false; //determine if the action is being perfromed at any given point

    public Action() //constructor
    {
        //declare preconditions and effects
        preconditions = new Dictionary<string, int>(); 
        effects = new Dictionary<string, int>();
    }

    public void Awake() //to initialize variables and required states before actions are used (avoid null)
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>(); //get hold of the NavMesh agent performing the action

        //populate the preconditions and effects dictionaries with the values popluated from the inpsector by being added the preConditions and afterEffects
        if (preConditions != null) //ensure that they are set in the inspector to avoid a crash at runtime
            foreach (EnvironmentState w in preConditions) //for every action precondition 
            {
                preconditions.Add(w.key, w.value); //add it to the preconditions dictionary for use in the planner
            }
        //similary done for the effects
        if (afterEffects != null)
            foreach (EnvironmentState w in afterEffects) //for every action after effect
            {
                effects.Add(w.key, w.value); //add it to the actions dictionary for use in the planner
            }

        inventory = this.GetComponent<Agent>().inventory; //get the resources available to the agent
        beliefs = this.GetComponent<Agent>().beliefs; //give the action access to the environment states the agent knows about  (agent instant that is performing "this" action)
    }

    public bool IsAchievable() //helper method for the planner to determine that an action can be performed
    {
        return true;
    }

    public bool IsAchievableGiven(Dictionary<string, int> conditions) //determine if action can be performed given that there is a certain precondition that must be satisfied
    {
        foreach (KeyValuePair<string, int> p in preconditions) //checking through preconditions stored for this particular action and looking for matching preconditions for the conditions passed as a parameter
        {
            if (!conditions.ContainsKey(p.Key)) //if no matching conditions are found, then the actions cannot be perfromed (as based on a GOAP system)
                return false; //hence, false is returned
        }
        return true; //return true when matching conditions are found which signify that the action can be perfromed
    }

    //abstract methods to force the use of Pre and Post performance methods allow the use of customized code for each action and other checks are done before each action starts (such as ensuring other resrouces or agents as available, and resulting condiitoins that might affect the environment state or condition)
    public abstract bool PrePerform(); 
    public abstract bool PostPerform(); 
}
