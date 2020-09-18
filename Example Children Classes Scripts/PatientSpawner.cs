using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientSpawner : MonoBehaviour //script to spawn patients outside clinic (at the start and throughout the simulation)
{
    public GameObject patientPrefab; //get access to the patient prefab 
    public int numberOfPatients; //specify the number of patients that will be spawned

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < numberOfPatients; i++) //loop to initially spawn a specified number of paitnets (specified by numberOfPatients) at spawner game object to start the simulation
        {
            Instantiate(patientPrefab, this.transform.position, Quaternion.identity); //instantiate creates the patient agents at the spawner's position given the patient prefab
        }

        Invoke("SpawnPatient", 5); //spawn patient method invoked to repeatedly spawn new patients (delay invoke every 5 seconds)
    } 

    void SpawnPatient() //infinite recursive method to spawn patients (recursion technique of invoking itself)
    {
        Instantiate(patientPrefab, this.transform.position, Quaternion.identity); //instantiate creates the patient agents at the spawner's position (spawned facing the default direction the spawner game object is facing)
        Invoke("SpawnPatient", Random.Range(8,10)); //method invokes itself to spawn patients every at random intervals between 2 to 10 seconds
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
