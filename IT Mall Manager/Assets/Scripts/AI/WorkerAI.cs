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
    private List<Collider> emptyShelves = new List<Collider>();
    public Collider superContainerCollider;

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
                    StartCoroutine(MoveToShelf(shelfToRestock.transform.position));
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
                MoveToSuperContainer();
                yield return new WaitUntil(() => isCarryingBox);
            }

            if (isCarryingBox)
            {
                Collider shelfToRestock = FindShelfToRestock();
                if (shelfToRestock != null)
                {
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

    private IEnumerator MoveToShelf(Vector3 position)
    {
        navMeshAgent.SetDestination(position);
        animator.SetBool("isMoving", true);
        yield return new WaitForSeconds(2f);
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
                return shelfCollider;
            }
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SuperContainer") && !isCarryingBox)
        {
            SuperContainer superContainer = superContainerCollider.GetComponentInParent<SuperContainer>();
            if (superContainer != null)
            {
                superContainer.MoveBoxToWorkerAi();

                if (superContainer.heldPackage != null)
                {
                    heldPackage = superContainer.heldPackage;
                    isCarryingBox = true;

                    FindEmptyShelves();
                }
            }
        }
        else if (other.CompareTag("Shelf") && isCarryingBox && allowedShelfTypes.Contains(other.GetComponentInParent<Shelf>().shelfType.ToString()))
        {
            Shelf shelf = other.GetComponentInParent<Shelf>();
            if (shelf != null)
            {
                RestockShelf(shelf);
                emptyShelves.Remove(other);
                if (emptyShelves.Count > 0)
                {
                    MoveToShelf(emptyShelves[0].transform.position);
                }
                else
                {
                    MoveToSuperContainer();
                }
            }
        }
    }

    private void RestockShelf(Shelf shelf)
    {
        ProductInfo productInfo = heldPackage.GetComponent<ProductInfo>();
        if (productInfo != null)
        {
            shelf.Restock(productInfo);
        }

        Destroy(heldPackage);
        heldPackage = null;
        isCarryingBox = false;
    }

    private void FindEmptyShelves()
    {
        emptyShelves.Clear();

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
                emptyShelves.Add(shelfCollider);
            }
        }

        if (emptyShelves.Count > 0)
        {
            MoveToShelf(emptyShelves[0].transform.position);
        }
        else
        {
            MoveToSuperContainer();
        }
    }
}
