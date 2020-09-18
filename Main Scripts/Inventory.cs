using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory //this class helps keep track of available resources for the agents (such as cubicles)
{
    public List<GameObject> resources = new List<GameObject>(); //list to store resrouces of environment and keep track of them
    
    public void AddResource(GameObject i) //add resource to the list 
    {
        resources.Add(i);
    }

    public GameObject FindResourceWithTag(string givenTag) //get a resrouce with the given tag from the scene 
    {
        foreach(GameObject resource in resources) //search trhogh found resources
        {
            if(resource.tag == givenTag) //if one is found with tag matching the given one
            {
                return resource; //return that resource
            }
        }
        return null; //if item is not found, nothing is returned
    }

    public void RemoveResource(GameObject givenResource) //remove resource from the list 
    {
        int indexOfResourceToRemove= -1; //will keep track of index for resrouce to remove (-1 to help determine if it was found or an issue occured)
        foreach(GameObject resource in resources)
        {
            indexOfResourceToRemove++; //update index as the list is progressed through
            if(resource == givenResource) // if that resource is found...
            {
                break; //exit from loop (index updated in loop)
            }

            if(indexOfResourceToRemove >= -1) //if the index is greated than -1, then no issues came up
            {
                resources.RemoveAt(indexOfResourceToRemove); //remove the resource from the list based on index tracked
            }
        }
    }
}
