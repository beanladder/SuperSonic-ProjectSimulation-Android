using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WorkerAI : MonoBehaviour
{
    public Transform workerAIHands;
    public List<string> allowedShelfTypes;
    public Collider superContainerCollider;
    public static WorkerAI instance;
    private NavMeshAgent navMeshAgent;
    public Animator animator;
    private bool waitingForShelf;
    public int AImaxProducts;
    public Vector3 WorkerDesiredRotation;
    private void Awake()
    {
        instance = this;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on " + gameObject.name);
        }

        if (animator == null)
        {
            Debug.LogError("Animator component is missing on " + gameObject.name);
        }
    }

    private void Start()
    {
        AImaxProducts = 1;
        StartCoroutine(WorkerRoutine());
    }

    private void Update()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }
    }

    private IEnumerator WorkerRoutine()
    {
        while (true)
        {
            yield return MoveToSuperContainer();
            yield return new WaitForSeconds(1f);

            yield return VisitAllowedShelves();

            if (workerAIHands.childCount == 0)
            {
                yield return MoveToSuperContainer();
            }
            else
            {
                yield return CheckShelfAvailability();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void Upgrade()
    {
        AImaxProducts++;
    }
    private IEnumerator MoveToSuperContainer()
    {
        navMeshAgent.SetDestination(superContainerCollider.transform.position);
        animator.SetBool("isMoving", true);
        yield return new WaitUntil(() => navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance);
    }

    private IEnumerator VisitAllowedShelves()
    {
        GameObject[] shelfObjects = GameObject.FindGameObjectsWithTag("Shelf");
        Collider[] shelves = shelfObjects
            .Select(obj => obj.GetComponent<Collider>())
            .Where(col => col != null && allowedShelfTypes.Contains(col.GetComponentInParent<Shelf>().shelfType.ToString()))
            .ToArray();

        foreach (Collider shelfCollider in shelves)
        {
            navMeshAgent.SetDestination(shelfCollider.transform.position);
            animator.SetBool("isMoving", true);
            yield return new WaitUntil(() => navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance);

            if (workerAIHands.childCount == 0)
            {
                yield break; // Exit the loop if no box is in the worker's hands
            }
        }
    }

    private IEnumerator CheckShelfAvailability()
    {
        while (true)
        {
            if (workerAIHands.childCount == 0)
            {
                yield break;
            }

            GameObject[] shelfObjects = GameObject.FindGameObjectsWithTag("Shelf");
            Shelf[] shelves = shelfObjects
                .Select(obj => obj.GetComponentInParent<Shelf>())
                .Where(shelf => allowedShelfTypes.Contains(shelf.shelfType.ToString()))
                .ToArray();

            bool anyShelfAvailable = shelves.Any(shelf => shelf.productCount <= 3);

            if (anyShelfAvailable)
            {
                yield return VisitAllowedShelves();
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SuperContainer") && workerAIHands.childCount == 0)
        {
            SuperContainer superContainer = superContainerCollider.GetComponentInParent<SuperContainer>();
            if (superContainer != null)
            {
                superContainer.MoveBoxToWorkerAI();
            }
        }
    }
}
