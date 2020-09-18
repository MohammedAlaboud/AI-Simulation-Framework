using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitClinic : Action //for this action (which belongs to the patient) everything is set on the inspector
{
    //overriding methods from parent class
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        Destroy(this.gameObject); //patient agent instancned are removed from the scene once they are not needed anymore to avoid having too many issues
        return true;
    }
}
