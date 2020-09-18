using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTreated : Action
{
    public override bool PrePerform() //overriding methods from parent class
    {
        target = inventory.FindResourceWithTag("Cubicle"); //get access to a cubicle object in this agent's inventory (nurse would have given the patient access to the cubicle by this point from GetPatient Action)
        if(target == null)    //if there are issues getting access to the cubicle resrouce 
        {
            return false; //signal that an issue occured so that this action is aborted or repeated 
        }

        return true; //signal that the patient can get treated with no issues
    }

    public override bool PostPerform()
    {
        Environment.Instance.GetWorld().ModifyState("Treated", 1); //update environment instance to keep track of number of patients that are treated for future development (initally the doctor was used to treat patients but instead the nurse is used for this action to make the simulation run more smoothly)
        beliefs.ModifyState("isCured", 1); //update agent state as cured (on itself)
        inventory.RemoveResource(target); //remove cubicle from patient agent's own inventory after getting treated
        return true; //signal that this method worked
    }
}
