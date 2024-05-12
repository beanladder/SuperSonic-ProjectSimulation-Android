using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;



public class AINPC : MonoBehaviour
{
    public enum ProductType { Motherboard, CPU, RAM }

    public ProductType productType;
    private bool isTakingProduct = false;
    private bool isFinishedShopping = false;

    public int linePosition = -1; // The assigned line position for this NPC
    public bool isCheckoutDone = false;

    private Transform myQueuePosition; // The assigned queue position for this NPC

    private void Start()
    {
        productType = (ProductType)Random.Range(0, System.Enum.GetValues(typeof(ProductType)).Length);
        SetDestination();
    }

   private void Update()
    {
        // Check if the checkout is done and move accordingly
        if (isFinishedShopping && !isCheckoutDone)
        {
            // Move towards the checkout position
            MoveToCheckout(QueueManager.instance.queuePositions[linePosition].position);
        }
    }

    public void SetDestination()
    {
        // Get all objects with the "Shelf" tag
        GameObject[] shelves = GameObject.FindGameObjectsWithTag("Shelf");

        if (shelves.Length > 0)
        {
            // Choose a random shelf destination
            Transform destination = shelves[Random.Range(0, shelves.Length)].transform;
            GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(destination.position);
        }
        else
        {
            Debug.LogWarning("No shelves found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shelf"))
        {
            if (!isTakingProduct)
            {
                StartCoroutine(TakeProduct());
            }
        }
        else if (other.CompareTag("Checkout"))
        {
            StartCoroutine(Checkout());
        }
    }

    IEnumerator TakeProduct()
    {
        isTakingProduct = true;
        yield return new WaitForSeconds(4f);
        isFinishedShopping = true;
        QueueManager.instance.AddToQueue(this); // Add this NPC to the queue
    }

   IEnumerator Checkout()
    {
        yield return new WaitForSeconds(6f);

        Debug.Log("Checkout Function");

        // Let the QueueManager know that this NPC's checkout is done
        isCheckoutDone = true;
        QueueManager.instance.RemoveFromQueue(this); 
        // Move to a random spawn point before destroying itself
        MoveToRandomSpawnPoint();
    }

    private void MoveToRandomSpawnPoint()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPoints.Length > 0)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
            GetComponent<NavMeshAgent>().SetDestination(randomSpawnPoint.position);
            Destroy(gameObject, 15f); // Destroy after 5 seconds
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
}

