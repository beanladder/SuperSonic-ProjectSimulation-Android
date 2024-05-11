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
    private bool isCheckoutDone = false;

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
            MoveToCheckout();
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
        Debug.Log(gameObject.name + " picked " + productType.ToString());

        isTakingProduct = false; // Reset the flag

        // Let the QueueManager know that this NPC has finished shopping
        isFinishedShopping = true;
        // Claim the next available empty position in the queue for this NPC
        myQueuePosition = QueueManager.instance.GetNextEmptyPosition(transform);

    }

    IEnumerator Checkout()
    {
        yield return new WaitForSeconds(4f);

        Debug.Log("Checkout Function");

        // Let the QueueManager know that this NPC's checkout is done
        isCheckoutDone = true;

        QueueManager.instance.RemoveFromQueue(transform);
        Destroy(gameObject, 2f);
        
    }

    // Move towards the assigned position in the queue
    public void MoveToCheckout()
    {
        
        if (myQueuePosition != null)
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(myQueuePosition.position);
        }
        else
        {
            Debug.LogWarning("No queue position assigned for NPC.");
        }
    }
}

