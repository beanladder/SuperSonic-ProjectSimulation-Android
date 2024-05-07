using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINPC : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    public Animator anim;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        SetNextWaypoint();
    }

    void SetNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void Update()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            SetNextWaypoint();
        }

        // Steer towards the next waypoint
        Vector3 targetDir = waypoints[currentWaypointIndex].position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Move towards the next waypoint
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        if (distanceToWaypoint > navMeshAgent.stoppingDistance)
        {
            anim.SetBool("IsMoving", true);
            navMeshAgent.Move(transform.forward * moveSpeed * Time.deltaTime);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }
    }
}
