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
    private bool isInQueue = false;

    void Start()
    {
        productType = (ProductType)Random.Range(0, System.Enum.GetValues(typeof(ProductType)).Length);
        SetDestination();
    }

    void SetDestination()
    {
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
        else if (isTakingProduct && other.CompareTag("Line"))
        {
            if (!isInQueue)
            {
                JoinQueue();
            }
        }
        else if (isInQueue && other.CompareTag("Checkout"))
        {
            StartCoroutine(Checkout());
        }
    }

    IEnumerator TakeProduct()
    {
        isTakingProduct = true;
        yield return new WaitForSeconds(4f);
        Debug.Log(gameObject.name + " picked " + productType.ToString());
        MoveToQueue();
    }

    void JoinQueue()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
        isInQueue = true;
        QueueManager.Instance.AddToQueue(transform);
    }

    void MoveToQueue()
    {

        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(QueueManager.Instance.GetNextQueuePosition().position);
    }

    IEnumerator Checkout()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
        yield return new WaitForSeconds(4f);
        MoveToStartingPosition();
    }

    void MoveToStartingPosition()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(transform.position);
       Destroy(gameObject, 2f); // Destroy NPC after returning to starting position
    }
}
