using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rest : Action //for this action (which belongs to the nurse) movement and conditions set in the inspector
{
    //overriding methods from parent class
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        beliefs.RemoveState("exhausted"); //when resting action is done by nurse, that nurse is no longer exhausted
        return true;
    }
}
