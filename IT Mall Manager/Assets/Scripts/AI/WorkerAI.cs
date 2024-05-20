using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WorkerAI : MonoBehaviour
{
    public Transform workerAIHands;
    public int maxProducts;
    public List<string> allowedShelfTypes;
    public Collider superContainerCollider; // Collider for the SuperContainer

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool isCarryingBox = false;
    private GameObject heldPackage;

    private void Awake()
    {
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
        StartCoroutine(WorkerRoutine());
    }

    private void Update()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            animator.SetBool("isMoving", false);

            if (isCarryingBox)
            {
                Collider shelfToRestock = FindShelfToRestock();
                if (shelfToRestock != null)
                {
                    MoveToShelf(shelfToRestock.transform.position);
                }
            }
        }
    }

    private IEnumerator WorkerRoutine()
    {
        while (true)
        {
            if (!isCarryingBox)
            {
                Debug.Log("Worker is moving to SuperContainer.");
                MoveToSuperContainer();
                yield return new WaitUntil(() => isCarryingBox);
            }

            if (isCarryingBox)
            {
                Debug.Log("Worker is carrying a box, looking for a shelf to restock.");
                Collider shelfToRestock = FindShelfToRestock();
                if (shelfToRestock != null)
                {
                    Debug.Log("Found a shelf to restock.");
                    MoveToShelf(shelfToRestock.transform.position);
                }
                yield return new WaitUntil(() => !isCarryingBox);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void MoveToSuperContainer()
    {
        if (superContainerCollider == null)
        {
            Debug.LogError("SuperContainer collider reference is missing on " + gameObject.name);
            return;
        }

        navMeshAgent.SetDestination(superContainerCollider.transform.position);
        animator.SetBool("isMoving", true);
    }

    private void MoveToShelf(Vector3 position)
    {
        navMeshAgent.SetDestination(position);
        animator.SetBool("isMoving", true);
    }

    private Collider FindShelfToRestock()
    {
        GameObject[] shelfObjects = GameObject.FindGameObjectsWithTag("Shelf");
        Collider[] shelves = shelfObjects
            .Select(obj => obj.GetComponent<Collider>())
            .Where(col => col != null && allowedShelfTypes.Contains(col.GetComponentInParent<Shelf>().shelfType.ToString()))
            .ToArray();

        foreach (Collider shelfCollider in shelves)
        {
            Shelf shelf = shelfCollider.GetComponentInParent<Shelf>();
            if (shelf != null && shelf.productCount < maxProducts)
            {
                Debug.Log("Found empty Shelf : " + shelfCollider.name);
                return shelfCollider;
            }
        }

        Debug.Log("No suitable shelves found.");
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Worker triggered with: " + other.gameObject.name);
        
        if (other.CompareTag("SuperContainer") && !isCarryingBox)
        {
            Debug.Log("Worker reached SuperContainer.");
            SuperContainer superContainer = superContainerCollider.GetComponentInParent<SuperContainer>();
            if (superContainer != null)
            {
                Debug.Log("Calling MoveBoxToWorkerAi from SuperContainer.");
                superContainer.MoveBoxToWorkerAi();
                
                if (superContainer.heldPackage != null)
                {
                    Debug.Log("Worker picked up a box: " + superContainer.heldPackage.name);
                    heldPackage = superContainer.heldPackage;
                    isCarryingBox = true;

                    // Immediately find a shelf to restock
                    Collider shelfToRestock = FindShelfToRestock();
                    if (shelfToRestock != null)
                    {
                        MoveToShelf(shelfToRestock.transform.position);
                    }
                }
                else
                {
                    Debug.LogWarning("No package found in SuperContainer.");
                }
            }
            else
            {
                Debug.LogWarning("SuperContainer component not found on SuperContainer collider's parent.");
            }
        }
        else if (other.CompareTag("Shelf") && isCarryingBox && allowedShelfTypes.Contains(other.GetComponentInParent<Shelf>().shelfType.ToString()))
        {
            Shelf shelf = other.GetComponentInParent<Shelf>();
            if (shelf != null)
            {
                Debug.Log("Worker reached a shelf to restock.");
                RestockShelf(shelf);
            }
        }
    }

    private void RestockShelf(Shelf shelf)
    {
        Debug.Log("Restocking shelf: " + shelf.name);
        ProductInfo productInfo = heldPackage.GetComponent<ProductInfo>();
        if (productInfo != null)
        {
            shelf.Restock(productInfo);
        }

        Destroy(heldPackage);
        heldPackage = null;
        isCarryingBox = false;
    }
}
