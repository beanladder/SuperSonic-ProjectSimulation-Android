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
    public string storeName; // Store name to determine which queue to join

    private bool isTakingProduct = false;
    private bool isFinishedShopping = false;
    private bool isIrritated = false;
    private bool shelfChecking = true;

    private NavMeshAgent navMeshAgent;
    [SerializeField] float time;
    float timeDelay = 70f;
    private Animator animator;
    private Collider lastVisitedShelf;
    private PoppingAnimation pop;
    private NPCSpawner spawner; // Reference to the spawner that created this NPC

    private int requiredProducts = 1; // Reduced the number of products the NPC needs to buy to 1

    private void Awake()
    {
        pop = GetComponentInChildren<PoppingAnimation>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

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
        // Check the number of active shelves
        Collider[] shelfColliders = GameObject.FindGameObjectsWithTag("Shelf")
            .Select(obj => obj.GetComponent<Collider>())
            .Where(col => col != null && col.GetComponentInParent<Shelf>() != null && col.GetComponentInParent<Shelf>().gameObject.activeSelf && allowedShelfTypes.Contains(col.GetComponentInParent<Shelf>().shelfType.ToString()))
            .ToArray();

        // If there are less than 2 active shelves, only require 1 product
        if (shelfColliders.Length < 2)
        {
            requiredProducts = 1;
        }

        SetDestination();
    }

    private void Update()
    {
        time = time + 1f * Time.deltaTime;
        if(numOfProductsCarrying == 0 && time>timeDelay)
        {
            MoveToRandomSpawnPoint();
        }

        if (isFinishedShopping && !isCheckoutDone)
        {
            if (linePosition <= 15)
            {
                MoveToCheckout(QueueManager.instances[storeName].queuePositions[linePosition].position);
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

        if(numOfProductsCarrying>=1)
        {
            pop.HideEmote(pop.emoteGameObjects[0]);
        }
    }

    public void SetDestination()
    {
        animator.SetBool("isMoving", true);

        Collider[] shelfColliders = GameObject.FindGameObjectsWithTag("Shelf")
            .Select(obj => obj.GetComponent<Collider>())
            .Where(col => col != null && col.GetComponentInParent<Shelf>() != null && col.GetComponentInParent<Shelf>().gameObject.activeSelf && allowedShelfTypes.Contains(col.GetComponentInParent<Shelf>().shelfType.ToString()))
            .Where(col => col != lastVisitedShelf) // Avoid the last visited shelf
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
            Debug.LogWarning("No enabled shelves found with allowed shelf types or all shelves have been visited!");
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
            if (shelf != null)
            {
                Debug.Log($"Checking shelf: {shelf.name}, productCount: {shelf.productCount}");
                if (shelf.productCount > 0 && shelf.RemoveProduct())
                {
                    pop.HideEmote(pop.emoteGameObjects[0]);
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(TakeProduct(shelf));
                    yield break;
                }
                pop.ShowEmote(pop.emoteGameObjects[0]);
            }
            
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
        isTakingProduct = true;
        yield return new WaitForSeconds(5f); // Increased time to take a product by 2 more seconds

        numOfProductsCarrying++;
        productNames.Add(shelf.shelfType.ToString());

        isTakingProduct = false;
        shelfChecking = true;

        if (numOfProductsCarrying == requiredProducts)
        {
            isFinishedShopping = true;
            QueueManager.instances[storeName].AddToQueue(this); // Add to specific store's queue
        }
        else
        {
            SetDestination();
        }
    }

    private IEnumerator Checkout()
    {
        yield return new WaitForSeconds(taskTime * numOfProductsCarrying);

        isCheckoutDone = true;
        QueueManager.instances[storeName].RemoveFromQueue(this); // Remove from specific store's queue

        // Find the appropriate CashOutflow instance
        CashOutflow cashOutflow = FindStoreCashOutflow();
        if (cashOutflow != null)
        {
            StartCoroutine(cashOutflow.CashSpawn(cashOutflow.CashDeliveryTime));
        }
        
        MoveToRandomSpawnPoint();
        yield return new WaitForSeconds(4f);
        pop.ShowEmote(pop.emoteGameObjects[1]);
        yield return new WaitForSeconds(1f);
        pop.HideEmote(pop.emoteGameObjects[0]);
    }

    public void MoveToRandomSpawnPoint()
    {
        animator.SetBool("isMoving", true);
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPoints.Length > 0)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
            navMeshAgent.SetDestination(randomSpawnPoint.position);
            if (spawner != null)
            {
                spawner.NPCDestroyed();
            }
            Destroy(gameObject, 25f); // Destroy after 25 seconds
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

    private void OnEnable()
    {
        StartCoroutine(DelayedDestination());
    }

    private IEnumerator DelayedDestination()
    {
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed
        SetDestination();
    }

    private CashOutflow FindStoreCashOutflow()
    {
        // Find the correct CashOutflow instance based on the store name
        GameObject store = GameObject.Find(storeName);
        if (store != null)
        {
            return store.GetComponentInChildren<CashOutflow>();
        }
        else
        {
            Debug.LogWarning("Store not found for cash outflow!");
            return null;
        }
    }

    public void SetSpawner(NPCSpawner spawner)
    {
        this.spawner = spawner;
    }
}



