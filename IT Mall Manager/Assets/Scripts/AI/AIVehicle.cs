using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIVehicle : MonoBehaviour
{

    public Transform[] waypoints;
    public string waypointTag; // Add waypointTag property to store the assigned waypoint tag
    private NavMeshAgent navMeshAgent;
    private int currentWaypointIndex = 0;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetNextWaypoint();
    }

    void SetNextWaypoint()
    {
        Debug.Log("Moving to Next Destination");
        if (waypoints.Length == 0) return;
        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void Update()
    {
        // Check if reached the current waypoint
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            Debug.Log("Reached Destination");
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            SetNextWaypoint();
        }
    }
}
