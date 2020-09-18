using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Environment  //not using MonoBehaviour as this will act as a pure C# class for handling data rather than a script from which every unity class derives
{//additionally a sealed class will be used to help when using queues to prevent conflicts
    // Start is called before the first frame update

    private static readonly Environment instance = new Environment(); //treating the enviornment as a singleton so that only one version of the simulation's evironment exists at any given point to avoid copies with conflicting values 
    private static EnvironmentStates environment; //this refers to the dictionary of the EnvironmentState class which holds all the enviornmnet states

    private static Queue<GameObject> patientsWaitingQ; //a qeueue for nurses to help determine the order in which patients enter the waiting room
    private static Queue<GameObject> cubiclesAvailableQ; //a qeueue to update the environment to determine the available cubicles 


    static Environment() //constructor 
    {
        environment = new EnvironmentStates(); //setup the enviornment states list (as an EnvironmentState type that holds the dictionary of states)
        patientsWaitingQ = new Queue<GameObject>(); //initialising the patient queue for use
        cubiclesAvailableQ = new Queue<GameObject>(); //initialising the cubicles queue for use (this needs to be populated by the ones that exist in the world)

        GameObject[] cubicles = GameObject.FindGameObjectsWithTag("Cubicle"); //find the cubicles in the scene and store them in this list
        foreach(GameObject cubicle in cubicles) //populate the cubcicles queue with cubicles found in scene
        {
            cubiclesAvailableQ.Enqueue(cubicle);
        }

        if (cubicles.Length > 0) //if cubicles are found in the world
        {
            environment.ModifyState("FreeCubicle", cubicles.Length); //update the environment states based on how many are available
        }

        Time.timeScale = 3; //speed up simulation (CHANGE WHEN TESTING)
    }

    private Environment() //setup for using this class as a singleton 
    {
    }

    public void AddPatient(GameObject p) //enqueue patients to the patientsWaitingQ as they enter the waiting room
    {
        patientsWaitingQ.Enqueue(p); //add given patient to the queue
    }

    public GameObject RemovePatient() //dequeue patients who have been picked up by the nurses and return that patient that got picked up
    {
        if(patientsWaitingQ.Count == 0) //nothing to return when there are no patients in the waiting room
        {
            return null;
        }
        return patientsWaitingQ.Dequeue(); //otherwise get the patient at the front of the queue
    }

    public void AddCubicle(GameObject p) //enqueue cubicle to the cubiclesAvailableQ as they become available
    {
        cubiclesAvailableQ.Enqueue(p); //add given patient to the queue
    }

    public GameObject RemoveCubicle() //dequeue cublicles that are occupied
    {
        if (cubiclesAvailableQ.Count == 0) //nothing to return when there are no cubicles in the environment
        {
            return null;
        }
        return cubiclesAvailableQ.Dequeue(); //otherwise get the available cubicle
    }

    public static Environment Instance //initialisation for the single occurence of the environment 
    {
        get { return instance; }
    }

    public EnvironmentStates GetWorld() //getter method to return the enviornment 
    {
        return environment;
    }
}
