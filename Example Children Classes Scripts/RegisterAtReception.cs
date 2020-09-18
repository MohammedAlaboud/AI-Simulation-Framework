using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterAtReception : Action
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
