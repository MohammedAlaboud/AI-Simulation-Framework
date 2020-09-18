using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToWaitingRoom : Action
{
    //overriding methods from parent class
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        Environment.Instance.GetWorld().ModifyState("Waiting", 1); //patient agent updates the enviornment state when getting to the waiting room so that the nurse agent would know that it is possible to get a patient
        Environment.Instance.AddPatient(this.gameObject); //patient agent adds itself to the queue of patients waiting to be picked up by the nurse to maintain order
        beliefs.ModifyState("atHospital", 1); //agent updates environment state to signal that an additional agent is at the hospital ("atHospital" is used as a precondition for GetTreated action)
        return true;
    }
}
