using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINPC : MonoBehaviour
{
    public enum ProductType { Motherboard, CPU, RAM }

    public ProductType productType;
    public Transform checkoutCounter;
    public Transform[] shelfDestinations;
    private bool isTakingProduct = false;
    private bool isFinishedShopping = false;

    void Start()
    {
        productType = (ProductType)Random.Range(0, System.Enum.GetValues(typeof(ProductType)).Length);
        SetDestination();
    }

    public void SetDestination()
    {
        // Set initial destination to shelf
        int randomIndex = Random.Range(0, shelfDestinations.Length);
        Transform destination = shelfDestinations[randomIndex];
        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(destination.position);
    }

    void OnTriggerEnter(Collider other)
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
        isFinishedShopping = true;
        // Let the QueueManager know that this NPC has finished shopping
        QueueManager.Instance.AddToQueue();
        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(QueueManager.Instance.queuePositions[QueueManager.Instance.nextPositionIndex].position);
    }

    IEnumerator Checkout()
    {
        yield return new WaitForSeconds(4f);
        MoveToStartingPosition();
    }

    void MoveToStartingPosition()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(checkoutCounter.position);
        Destroy(gameObject, 2f); // Destroy NPC after returning to starting position
    }
}
