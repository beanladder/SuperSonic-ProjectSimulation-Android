using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;



public class AINPC : MonoBehaviour
{
    public List<string> allowedShelfTypes; // Shelf types that the NPC is allowed to visit

    private bool isTakingProduct = false;
    private bool isFinishedShopping = false;
    private bool isIrritated = false;

    private NavMeshAgent navmeshAgent;

    public int taskTime;
    public int numOfProductsCarrying;
    public List<string> productnames;
    public int linePosition = -1; // The assigned line position for this NPC
    public bool isCheckoutDone = false;
    public bool ShelfChecking = true;

    Animator animator;

    private Collider lastVisitedShelf; // Track the last visited shelf collider

    private void Awake() {
        animator = GetComponent<Animator>();
        navmeshAgent = GetComponent<NavMeshAgent>();

        if (navmeshAgent == null)
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
        // Check if the checkout is done and move accordingly
        if (isFinishedShopping && !isCheckoutDone)
        {
            if (linePosition <= 15)
            {
                // Move towards the checkout position
                MoveToCheckout(QueueManager.instance.queuePositions[linePosition].position);
            }
            else if (!isIrritated)
            {
                StartCoroutine(WaitforLine());
            }
        }
        
        if (navmeshAgent.remainingDistance <= navmeshAgent.stoppingDistance)
        {
            animator.SetBool("isMoving", false);
        }
    }

    public void SetDestination()
    {
        animator.SetBool("isMoving", true);

        // Find all colliders with the "Shelf" tag and filter them based on allowed shelf types
        Collider[] shelfColliders = GameObject.FindGameObjectsWithTag("Shelf")
            .Select(obj => obj.GetComponent<Collider>())
            .Where(col => col != lastVisitedShelf) // Exclude the last visited shelf
            .Where(col => allowedShelfTypes.Contains(col.GetComponentInParent<Shelf>().shelfType.ToString())) // Filter by allowed shelf types
            .ToArray();

        if (shelfColliders.Length > 0)
        {
            // Choose a random collider from the filtered list
            Collider randomCollider = shelfColliders[Random.Range(0, shelfColliders.Length)];

            // Update the last visited shelf
            lastVisitedShelf = randomCollider;

            // Generate a random point within the collider's bounds
            Vector3 randomPoint = GetRandomPointInCollider(randomCollider);

            // Set the destination to the random point
            navmeshAgent.SetDestination(randomPoint);
        }
        else
        {
            Debug.LogWarning("No shelves found with allowed shelf types!");
            animator.SetBool("isMoving", false);
        }
    }

    private Vector3 GetRandomPointInCollider(Collider collider)
    {
        // Get the bounds of the collider
        Bounds bounds = collider.bounds;

        // Generate a random point within the bounds
        Vector3 randomPoint = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );

        return randomPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shelf"))
        {
            if (!isTakingProduct && ShelfChecking)
            {
                StartCoroutine(Decision());
            }
        }
        else if (other.CompareTag("Checkout"))
        {
            StartCoroutine(Checkout(numOfProductsCarrying));
        }
    }

    public IEnumerator Decision()
    {
        ShelfChecking = false;
        int randomTime = Random.Range(5, 10);
        yield return new WaitForSeconds(randomTime);
        int chance = Random.Range(1, 15);

        if (numOfProductsCarrying == 1)
        {
            // NPC has one product, decide whether to buy more or proceed to checkout
            if (chance <= 7)
            {
                // Continue shopping
                StartCoroutine(TakeProduct());
            }
            else
            {
                if (linePosition <= 15)
                {
                    isFinishedShopping = true;
                    QueueManager.instance.AddToQueue(this);
                }
            }
        }
        else
        {
            // NPC has more than one product or none, continue normal decision-making
            if (chance > 0 && chance <= 7)
            {
                StartCoroutine(TakeProduct());
            }
            else if (chance > 7 && chance <= 9 && numOfProductsCarrying == 0)
            {
                MoveToRandomSpawnPoint();
            }
            else if (chance >= 10 && chance <= 15)
            {
                ShelfChecking = true;
                SetDestination();
            }
        }
    }

    IEnumerator TakeProduct()
    {
        isTakingProduct = true;
        bool shelfRestocked = false;
        float waitTime = 0f;

        // Check if the NPC has 0 products
        if (numOfProductsCarrying == 0)
        {
            // Wait for a short duration and periodically check if the shelf gets restocked
            while (waitTime < 15f)
            {
                Debug.Log("Waiting for restock");
                yield return new WaitForSeconds(1f);
                waitTime += 1f;

                // Check if the shelf is restocked
                if (lastVisitedShelf != null)
                {
                    Shelf shelf = lastVisitedShelf.GetComponentInParent<Shelf>();
                    if (shelf != null && shelf.productCount > 0)
                    {
                        // Shelf is restocked, take the product immediately
                        shelfRestocked = true;
                        break;
                    }
                }
            }

            // If the shelf is restocked, take the product immediately
            if (shelfRestocked)
            {
                Shelf shelf = lastVisitedShelf.GetComponentInParent<Shelf>();
                if (shelf != null && shelf.productCount > 0 && shelf.RemoveProduct())
                {
                    Debug.Log("Product taken from restocked shelf.");
                    numOfProductsCarrying++;
                    productnames.Add(shelf.shelfType.ToString());
                    //SpawnProductAtPoint(shelf.GetProductPrefab(), transform.position); // Spawn product at NPC's position

                    isFinishedShopping = true;
                    if (numOfProductsCarrying < 2)
                    {
                        Debug.Log("I wanna buy more");
                        isFinishedShopping = false;
                        ShelfChecking = true;
                        SetDestination();
                    }
                    if (linePosition <= 15 && isFinishedShopping)
                    {
                        QueueManager.instance.AddToQueue(this);
                    } // Add this NPC to the queue
                }
            }
            else
            {
                // Shelf was not restocked, move to another location
                Debug.Log("No products available on restocked shelf. Going home");
                MoveToRandomSpawnPoint();
            }
        }
        else
        {
            // NPC has more than 0 products, continue normal shopping process
            int chance = Random.Range(1, 10);
            if (lastVisitedShelf != null)
            {
                Shelf shelf = lastVisitedShelf.GetComponentInParent<Shelf>();
                if (shelf != null && shelf.productCount > 0 && shelf.RemoveProduct())
                {
                    Debug.Log("Product taken from shelf.");
                    numOfProductsCarrying++;
                    productnames.Add(shelf.shelfType.ToString());
                    //SpawnProductAtPoint(shelf.GetProductPrefab(), transform.position); // Spawn product at NPC's position

                    isFinishedShopping = true;
                    if (chance > 7 && numOfProductsCarrying < 2)
                    {
                        Debug.Log("I wanna buy more");
                        isFinishedShopping = false;
                        ShelfChecking = true;
                        SetDestination();
                    }
                    if (linePosition <= 15 && isFinishedShopping)
                    {
                        QueueManager.instance.AddToQueue(this);
                    } // Add this NPC to the queue
                }
                else if (numOfProductsCarrying == 0)
                {
                    ShelfChecking = false;
                    // If the NPC has no products and encounters an empty shelf, move to another location
                    Debug.Log("No products available on " + shelf.shelfType + " shelf. Going home");
                    MoveToRandomSpawnPoint();
                }
            }
            else
            {
                Debug.LogWarning("No last visited shelf found.");
            }
        }

        isTakingProduct = false;
    }

    IEnumerator Checkout(int productCount)
    {
        yield return new WaitForSeconds(taskTime * productCount);

        // Let the QueueManager know that this NPC's checkout is done
        isCheckoutDone = true;
        Debug.Log("I bought " + numOfProductsCarrying + " Products ");
        QueueManager.instance.RemoveFromQueue(this);
        CashOutflow.instance.StartCoroutine(CashOutflow.instance.CashSpawn(CashOutflow.instance.CashDeliveryTime));
        // Move to a random spawn point before destroying itself
        MoveToRandomSpawnPoint();
    }

    public void MoveToRandomSpawnPoint()
    {
        animator.SetBool("isMoving", true);
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPoints.Length > 0)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
            navmeshAgent.SetDestination(randomSpawnPoint.position);
            Destroy(gameObject, 15f); // Destroy after 15 seconds
            NPCSpawner.instance.NPCDestroyed();
        }
        else
        {
            Debug.LogWarning("No spawn points found!");
        }
    }

    // Move towards the assigned position in the queue
    public void MoveToCheckout(Vector3 position)
    {
        animator.SetBool("isMoving", true);
        navmeshAgent.SetDestination(position);
    }

    public IEnumerator WaitforLine()
    {
        isIrritated = true;
        Debug.Log(gameObject.name + " is Waiting for the line");
        yield return new WaitForSeconds(1f);

        MoveToRandomSpawnPoint();
    }

    // private void SpawnProductAtPoint(GameObject prefab, Vector3 position)
    // {
    //     if (prefab != null)
    //     {
    //         GameObject product = Instantiate(prefab, position, prefab.transform.rotation);
    //         product.transform.SetParent(transform); // Set the parent to the AINPC's transform
    //     }
    // }
}

