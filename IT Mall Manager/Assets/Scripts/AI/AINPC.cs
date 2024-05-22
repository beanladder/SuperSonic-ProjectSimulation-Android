using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;



public class AINPC : MonoBehaviour
{
    public List<string> allowedShelfTypes;
    public int taskTime;
    public int numOfProductsCarrying;
    public List<string> productNames;
    public int linePosition = -1;
    public bool isCheckoutDone = false;

    private bool isTakingProduct = false;
    private bool isFinishedShopping = false;
    private bool isIrritated = false;
    private bool shelfChecking = true;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Collider lastVisitedShelf;
    PoppingAnimation pop;

    private void Awake()
    {
        pop = GetComponentInChildren<PoppingAnimation>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on " + gameObject.name);
        }
    }

    private void Start()
    {
        SetDestination();
    }

    private void Update()
    {
        if (isFinishedShopping && !isCheckoutDone)
        {
            if (linePosition <= 15)
            {
                MoveToCheckout(QueueManager.instance.queuePositions[linePosition].position);
            }
            else if (!isIrritated)
            {
                StartCoroutine(WaitForLine());
            }
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            animator.SetBool("isMoving", false);
        }
    }

    public void SetDestination()
    {
        animator.SetBool("isMoving", true);

        Collider[] shelfColliders = GameObject.FindGameObjectsWithTag("Shelf")
            .Select(obj => obj.GetComponent<Collider>())
            .Where(col => col != lastVisitedShelf && allowedShelfTypes.Contains(col.GetComponentInParent<Shelf>().shelfType.ToString()))
            .ToArray();

        if (shelfColliders.Length > 0)
        {
            Collider randomCollider = shelfColliders[Random.Range(0, shelfColliders.Length)];
            lastVisitedShelf = randomCollider;
            Vector3 randomPoint = GetRandomPointInCollider(randomCollider);
            navMeshAgent.SetDestination(randomPoint);
        }
        else
        {
            Debug.LogWarning("No shelves found with allowed shelf types!");
            animator.SetBool("isMoving", false);
        }
    }

    private Vector3 GetRandomPointInCollider(Collider collider)
    {
        Bounds bounds = collider.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shelf") && !isTakingProduct && shelfChecking)
        {
            StartCoroutine(WaitForShelfStock(other.GetComponentInParent<Shelf>()));
        }
        else if (other.CompareTag("Checkout"))
        {
            StartCoroutine(Checkout());
        }
    }

    private IEnumerator WaitForShelfStock(Shelf shelf)
    {
        shelfChecking = false;
        float waitTime = 0f;

        while (waitTime < 15f)
        {
            if (shelf != null && shelf.productCount > 0 && shelf.RemoveProduct())
            {
                yield return new WaitForSeconds(1f);
                pop.PopOut();
                StartCoroutine(TakeProduct(shelf));
                yield break;
            }
            pop.PopIn();

            yield return new WaitForSeconds(1f);
            waitTime += 1f;
        }

        if (numOfProductsCarrying == 0)
        {
            MoveToRandomSpawnPoint();
        }
        else
        {
            SetDestination();
        }

        shelfChecking = true;
    }

    private IEnumerator TakeProduct(Shelf shelf)
    {
        yield return new WaitForSeconds(3f);
        isTakingProduct = true;

        if (numOfProductsCarrying < 2 )
        {
            numOfProductsCarrying++;
            productNames.Add(shelf.shelfType.ToString());

            if (numOfProductsCarrying == 2)
            {
                isFinishedShopping = true;
                QueueManager.instance.AddToQueue(this);
            }
            else
            {
                shelfChecking = true;
                SetDestination();
            }
        }
        else
        {
            MoveToRandomSpawnPoint();
        }

        isTakingProduct = false;
    }

    private IEnumerator Checkout()
    {
        yield return new WaitForSeconds(taskTime * numOfProductsCarrying);

        isCheckoutDone = true;
        QueueManager.instance.RemoveFromQueue(this);
        CashOutflow.instance.StartCoroutine(CashOutflow.instance.CashSpawn(CashOutflow.instance.CashDeliveryTime));
        MoveToRandomSpawnPoint();
    }

    public void MoveToRandomSpawnPoint()
    {
        animator.SetBool("isMoving", true);
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPoints.Length > 0)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
            navMeshAgent.SetDestination(randomSpawnPoint.position);

            // Return the NPC to the pool after 15 seconds
            Invoke("ReturnToPool", 15f);
        }
        else
        {
            Debug.LogWarning("No spawn points found!");
        }
    }

    private void ReturnToPool()
    {
        NPCSpawner.instance.NPCDestroyed();
        ObjectPool.instance.ReturnNPC(gameObject);
    }

    public void MoveToCheckout(Vector3 position)
    {
        animator.SetBool("isMoving", true);
        navMeshAgent.SetDestination(position);
    }

    private IEnumerator WaitForLine()
    {
        isIrritated = true;
        yield return new WaitForSeconds(1f);
        MoveToRandomSpawnPoint();
    }

    private void OnEnable()
    {
    StartCoroutine(DelayedDestination());
    }

    private IEnumerator DelayedDestination()
    {
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed
        SetDestination();
    }   
}



