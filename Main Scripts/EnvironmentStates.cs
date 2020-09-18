using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //EnvironmentState seriaized so that it can be accessible from the inspector
public class EnvironmentState //the purpose of this class is to have access to the key and value variables used as a dictionary in which the key is the world state and a value to associate it to the key
{
    //these can be accessed from other classes
    public string key; //the key identifies the state itself
    public int value; //the value here is used to determine the number of resources available for that state 
                      //For example, the value will increase in number when more of the resource for that state becomes available 
}

public class EnvironmentStates  //not using MonoBehaviour as this will act as a pure C# class for handling data rather than a script from which every unity class derives
{
    public Dictionary<string, int> states; //the list of states as a dictionary type

    public EnvironmentStates() //constructor 
    {
        states = new Dictionary<string, int>(); //setting up dictionary for class
    }

    //Below are the methods needed to use the dictionary:
    public bool HasState(string key) //check if a particlar state exists within the dictionary based on a given key 
    {
        return states.ContainsKey(key); //ContainsKey is part of the Dictionary class 
    }

    void AddState(string key, int value) //add a state to the dictionary 
    {
        states.Add(key, value);
    }

    public void RemoveState(string key) //remove a state from the dictionary after making sure that the state already exists to avoid errors
    {
        if (states.ContainsKey(key))
            states.Remove(key);
    }

    public void ModifyState(string key, int value) //used to modify the number of available resrouces for each state
    {
        if (states.ContainsKey(key)) //ensure that state being modified exists in the dictionary 
        {
            states[key] += value; //the state is modified by increasing the value associated with the key for that state
            if (states[key] <= 0) //if the value for that state (obtained by using the key) is more than zero, then that state is removed
            {   //will be using negative numbers here to signify the amount of resources available for that state  
                RemoveState(key);
            }  
        }
        else //otherwise if the state doesnt exist, then it will be added rather than being incremented
        {
            states.Add(key, value);
        }   
    }

    public void SetState(string key, int value) //to directly set a value for a state (using the key) This is used rather than modifying the value in increments in terms of availble resrouces
    {
        if (states.ContainsKey(key)) //ensure state already exists 
            states[key] = value; //change the value based on passed paramter
        else
            states.Add(key, value); //if the state does not already exist, then one is added with the modified value
    }

    public Dictionary<string, int> GetStates() //to return the complete dictionary of all states used when the planner needs to access all states in the environment
    {
        return states;
    }

}
