using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPatient : Action
{
    GameObject resource; //used to store the resouce that needs to be in the inventory; in this case the resource needed for getting a patient is a cubicle

    public override bool PrePerform() //overriding methods from parent class
    {
        target = Environment.Instance.RemovePatient(); //when nurse gets a patient, that patient is removed from the queue of patients waiting in the waiting room
        if(target == null)    //test for environment instance to avoid errors
        {
            return false; //if issues occur, nurse should attempt to repeat this action by having this action's goal still exist for the nurse
        }

        resource = Environment.Instance.RemoveCubicle(); //give the resource a pointer to the cubicle that is first in the queue of available cubicles 
        if(resource != null) //gaurd against null error (if the available cubicle exists) 
        {
            inventory.AddResource(resource);//add it to the nurse's inventory (the one performing the action)
        } 
        else
        {
            Environment.Instance.AddPatient(target); //if cannot get hold of an available cubicle, then release patient the nurse got
            target = null; //preperform failed
            return false; //signal that this action needs to be repeated 
        }

        Environment.Instance.GetWorld().ModifyState("FreeCubicle", -1);//when code reaches this part, that means a cubicle is in use and the environment needs to be updated (reduce count on that environment condition by 1)
        return true; //nurse agent would go ahead and perform the action if its in the right position
    }

    public override bool PostPerform()
    {
        Environment.Instance.GetWorld().ModifyState("Waiting", -1); //if action is complete, then the agent that the nurse gotten is no longer waiting and the environment needs to be updated to count one less waiting patient
        if(target) //if the patient the nurse was headed to exists (gaurd against errors, the agent must exist by this point)
        {
            target.GetComponent<Agent>().inventory.AddResource(resource); //the nurse gives that patient access to the cubicle that the nurse will guide the patient to
        }
        return true; //signal that this method worked
    }
}
