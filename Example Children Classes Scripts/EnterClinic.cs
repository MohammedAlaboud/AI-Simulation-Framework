using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterClinic : Action
{
    //overriding methods from parent class
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }
}
