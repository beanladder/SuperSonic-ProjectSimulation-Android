using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINPC : MonoBehaviour
{
    public enum ProductType { Motherboard, CPU, RAM }

    public Transform cashCounter;
    public Transform motherboardShelf;
    public Transform cpuShelf;
    public Transform ramShelf;

    public ProductType productType; // Variable to store the product type

    private NavMeshAgent navMeshAgent;
    private bool isTakingProduct = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // Choose a random product type
        productType = (ProductType)Random.Range(0, System.Enum.GetValues(typeof(ProductType)).Length);
        SetDestination();
    }

    void SetDestination()
    {
        // Choose a shelf based on product type
        switch (productType)
        {
            case ProductType.Motherboard:
                navMeshAgent.SetDestination(motherboardShelf.position);
                break;
            case ProductType.CPU:
                navMeshAgent.SetDestination(cpuShelf.position);
                break;
            case ProductType.RAM:
                navMeshAgent.SetDestination(ramShelf.position);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the AI NPC entered the shelf trigger
        if (other.CompareTag("Shelf"))
        {
            // Check if the AI NPC is already taking the product
            if (!isTakingProduct)
            {
                StartCoroutine(TakeProduct());
            }
        }
        // Check if the AI NPC entered the checkout trigger
        else if (other.CompareTag("Checkout"))
        {
            StartCoroutine(Checkout());
        }
    }

    public IEnumerator TakeProduct()
    {
        // Set flag to indicate that the AI NPC is taking the product
        isTakingProduct = true;

        // Stop moving
        navMeshAgent.isStopped = true;

        // Wait for 4 seconds to simulate taking the product
        yield return new WaitForSeconds(4f);

        // Log the product picked
        Debug.Log("AI picked " + productType.ToString());

        // Move towards the cash counter
        navMeshAgent.SetDestination(cashCounter.position);

        // Resume moving
        navMeshAgent.isStopped = false;

        // Reset flag
        isTakingProduct = false;
    }

    public IEnumerator Checkout()
    {
        // Stop moving
        navMeshAgent.isStopped = true;

        // Wait for 4 seconds to simulate checking out
        yield return new WaitForSeconds(4f);

        // Destroy the AI NPC after checking out
        Destroy(gameObject);
    }
}
