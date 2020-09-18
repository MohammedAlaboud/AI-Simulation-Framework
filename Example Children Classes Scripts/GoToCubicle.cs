using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToCubicle : Action
{ //action to make the nurse guide pateint to the cubicle to treat the patient 
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
        Environment.Instance.GetWorld().ModifyState("TreatingPatient", 1); //update environment instance to keep track of nurses treating patients 
        Environment.Instance.AddCubicle(target); //cubicle becomes free after nurse treats patient there (originally doctoragents were the ones treating patients but having three agents share resources caused delays and made the simulation less smooth)
        inventory.RemoveResource(target); //remove cubicle from nurse's agent's own inventory after treating
        Environment.Instance.GetWorld().ModifyState("FreeCubcile", 1); //update environment to ensure that the cubicle the nurse treated patient in is free after the action is done
        return true; //signal that this method worked
    }
}
