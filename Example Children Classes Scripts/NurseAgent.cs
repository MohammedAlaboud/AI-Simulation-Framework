using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseAgent : Agent //Nurse agent type inherits from agent class
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start(); //run the setup already made in the parent class

        //setup nurse's goals
        SubGoal s1 = new SubGoal("treatPatient", 1, false); //make sure this goal is repeated by setting the ability to remove this goal when its done to false
        goals.Add(s1, 3);

        SubGoal s2 = new SubGoal("rested", 1, false); //this is another goal that the nurse should repeat (simulation design choice)
        goals.Add(s2, 1);

        Invoke("GetTired", Random.Range(10,20)); //nurse agents take break at start of simulation
    }

    void GetTired() //method invoked randomly to force nurse to take a break (perform rest action)
    {
        beliefs.ModifyState("exhausted", 0); //nurse's state is exhausted 
        Invoke("GetTired", Random.Range(10,20)); //method invokes itself within a random time range
    }
}
