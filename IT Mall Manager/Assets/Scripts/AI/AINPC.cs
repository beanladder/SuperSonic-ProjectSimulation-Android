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

    public int taskTime = 6;
    public int linePosition = -1; // The assigned line position for this NPC
    public bool isCheckoutDone = false;
    public bool ShelfChecking = true;

    private Collider lastVisitedShelf; // Track the last visited shelf collider

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
    }

    public void SetDestination()
    {
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
            GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(randomPoint);
        }
        else
        {
            Debug.LogWarning("No shelves found with allowed shelf types!");
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
        if (other.CompareTag("Shelf") && ShelfChecking)
        {
            if (!isTakingProduct)
            {
                StartCoroutine(Decision());
            }
        }
        else if (other.CompareTag("Checkout"))
        {
            StartCoroutine(Checkout());
        }
    }

    public IEnumerator Decision()
    {
        ShelfChecking = false;
        int randomTime = Random.Range(5, 10);
        yield return new WaitForSeconds(randomTime);
        int chance = Random.Range(1, 15);
        if (chance > 0 && chance <= 7)
        {
            Debug.Log("Got my product");
            StartCoroutine(TakeProduct());
        }
        else if (chance > 7 && chance <= 10)
        {
            Debug.Log("No Product to Buy");
            MoveToRandomSpawnPoint();
        }
        else if (chance > 10 && chance <= 15)
        {
            ShelfChecking = true;
            Debug.Log("Need to search more");
            SetDestination();
        }
    }

    IEnumerator TakeProduct()
    {
        isTakingProduct = true;
        yield return new WaitForSeconds(4f);

        // Ensure lastVisitedShelf is not null
        if (lastVisitedShelf != null)
        {
            Shelf shelf = lastVisitedShelf.GetComponentInParent<Shelf>();
            if (shelf != null && shelf.RemoveProduct())
            {
                Debug.Log("Product taken from shelf.");
                isFinishedShopping = true;
                if (linePosition <= 15)
                {
                    QueueManager.instance.AddToQueue(this);
                } // Add this NPC to the queue
            }
            else
            {
                Debug.Log("Restock " + shelf.shelfType + " shelf else I will go");
                yield return new WaitForSeconds(10f);
                MoveToRandomSpawnPoint();
            }
        }
        else
        {
            Debug.LogWarning("No last visited shelf found.");
        }

        isTakingProduct = false;
    }

    IEnumerator Checkout()
    {
        yield return new WaitForSeconds(taskTime);

        Debug.Log("Checkout Function");

        // Let the QueueManager know that this NPC's checkout is done
        isCheckoutDone = true;
        QueueManager.instance.RemoveFromQueue(this);
        CashOutflow.instance.StartCoroutine(CashOutflow.instance.CashSpawn(CashOutflow.instance.CashDeliveryTime));
        // Move to a random spawn point before destroying itself
        MoveToRandomSpawnPoint();
    }

    public void MoveToRandomSpawnPoint()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPoints.Length > 0)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
            GetComponent<NavMeshAgent>().SetDestination(randomSpawnPoint.position);
            Destroy(gameObject, 15f); // Destroy after 5 seconds
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
        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(position);
    }

    public IEnumerator WaitforLine()
    {
        isIrritated = true;
        Debug.Log(gameObject.name + " is Waiting for the line");
        yield return new WaitForSeconds(1f);

        MoveToRandomSpawnPoint();
    }
}

