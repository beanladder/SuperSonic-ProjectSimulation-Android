using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWaypointAssigner : MonoBehaviour
{
    public GameObject[] carPrefabs; // Array of car prefabs
    public Transform spawnPoint; // Spawn point for cars
    public Transform[] upstreamWaypoints; // Waypoints for upstream cars
    public Transform[] downstreamWaypoints; // Waypoints for downstream cars

    public int maxCars = 10; // Maximum number of cars

    void Start()
    {
        StartCoroutine(CarSpawner(maxCars));
        
    }

    public IEnumerator CarSpawner(int max)
    { 
        for (int i = 0; i < max; i++)
        {
            SpawnCar();
            yield return new WaitForSeconds(4f);
        }
    }

    public void SpawnCar()
    {

        
        // Randomly select a car prefab
        GameObject selectedCarPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];

        // Instantiate the selected car prefab at the spawn point
        GameObject car = Instantiate(selectedCarPrefab, spawnPoint.position, Quaternion.identity);

        // Get the AIVehicle component
        AIVehicle aiVehicle = car.GetComponent<AIVehicle>();

        // Assign waypoints based on the car type
        if (Random.value < 0.5f)
        {
            aiVehicle.waypoints = upstreamWaypoints;
        }
        else
        {
            aiVehicle.waypoints = downstreamWaypoints;
        }

        
    }
}