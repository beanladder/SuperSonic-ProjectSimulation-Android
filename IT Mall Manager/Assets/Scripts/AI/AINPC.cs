using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;



public class AINPC : MonoBehaviour
{
    public List<string> allowedShelfTypes; // Shelf types that the NPC is allowed to visit
    public int taskTime;
    public int numOfProductsCarrying;
    public List<string> productNames;
    public int linePosition = -1; // The assigned line position for this NPC
    public bool isCheckoutDone = false;

    private bool isTakingProduct = false;
    private bool isFinishedShopping = false;
    private bool isIrritated = false;
    private bool shelfChecking = true;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Collider lastVisitedShelf; // Track the last visited shelf collider

    private void Awake()
    {
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
            StartCoroutine(Decision());
        }
        else if (other.CompareTag("Checkout"))
        {
            StartCoroutine(Checkout());
        }
    }

    private IEnumerator Decision()
    {
        shelfChecking = false;
        yield return new WaitForSeconds(Random.Range(5, 10));

        if (numOfProductsCarrying == 1)
        {
            if (Random.Range(1, 15) <= 7)
            {
                StartCoroutine(TakeProduct());
            }
            else if (linePosition <= 15)
            {
                isFinishedShopping = true;
                QueueManager.instance.AddToQueue(this);
            }
        }
        else
        {
            int chance = Random.Range(1, 15);
            if (chance <= 7)
            {
                StartCoroutine(TakeProduct());
            }
            else if (chance > 9 && numOfProductsCarrying == 0)
            {
                MoveToRandomSpawnPoint();
            }
            else
            {
                shelfChecking = true;
                SetDestination();
            }
        }
    }

    private IEnumerator TakeProduct()
    {
        isTakingProduct = true;
        float waitTime = 0f;
        bool shelfRestocked = false;

        if (numOfProductsCarrying == 0)
        {
            while (waitTime < 15f)
            {
                yield return new WaitForSeconds(1f);
                waitTime += 1f;

                if (lastVisitedShelf != null)
                {
                    Shelf shelf = lastVisitedShelf.GetComponentInParent<Shelf>();
                    if (shelf != null && shelf.productCount > 0)
                    {
                        shelfRestocked = true;
                        break;
                    }
                }
            }

            if (shelfRestocked)
            {
                Shelf shelf = lastVisitedShelf.GetComponentInParent<Shelf>();
                if (shelf != null && shelf.RemoveProduct())
                {
                    numOfProductsCarrying++;
                    productNames.Add(shelf.shelfType.ToString());
                    shelfChecking = true;
                    SetDestination();
                }
            }
            else
            {
                MoveToRandomSpawnPoint();
            }
        }
        else
        {
            Shelf shelf = lastVisitedShelf.GetComponentInParent<Shelf>();
            if (shelf != null && shelf.productCount > 0 && shelf.RemoveProduct())
            {
                numOfProductsCarrying++;
                productNames.Add(shelf.shelfType.ToString());

                if (Random.Range(1, 10) > 7 && numOfProductsCarrying < 2)
                {
                    shelfChecking = true;
                    SetDestination();
                }
                else if (linePosition <= 15)
                {
                    isFinishedShopping = true;
                    QueueManager.instance.AddToQueue(this);
                }
            }
            else if (numOfProductsCarrying == 0)
            {
                MoveToRandomSpawnPoint();
            }
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
            Destroy(gameObject, 15f); // Destroy after 15 seconds
            NPCSpawner.instance.NPCDestroyed();
        }
        else
        {
            Debug.LogWarning("No spawn points found!");
        }
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
}

