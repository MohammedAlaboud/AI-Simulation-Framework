using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientAgent : Agent //Patient agent type inherits from agent class
{
    // Start is called before the first frame update
    new void Start() //new keyword required for child casses to avoid warnings
    {
        base.Start(); //run the setup already made in the parent class

        //adding agent's subgoals which are used to lead the agent through the simulation
        SubGoal s1 = new SubGoal("isWaiting", 1, true);
        goals.Add(s1, 3);

        SubGoal s2 = new SubGoal("isTreated", 1, true);
        goals.Add(s2, 5);

        SubGoal s3 = new SubGoal("isHome", 1, true);
        goals.Add(s3, 5);
    }
    
}
