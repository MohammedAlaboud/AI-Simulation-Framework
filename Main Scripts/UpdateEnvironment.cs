using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateEnvironment : MonoBehaviour //this script is for debugging purposes to display world states on the game screen (a UI canvas is added to display this text onto)
{
    public Text states; //to store the text

    // Update is called once per frame
    void LateUpdate() //does not need to be done to often
    {
        Dictionary<string, int> environmentStates = Environment.Instance.GetWorld().GetStates(); //access the instace of the evironment to get the environment states
        states.text = "";
        foreach(KeyValuePair<string, int> state in environmentStates) //go through the world states and for each one...
        {
            states.text += state.Key + ", " + state.Value + "\n"; //update the text to store the state and how many of those states exist in the world (for example, how many patients are waiting in the waiting room)
        }
    }
}
