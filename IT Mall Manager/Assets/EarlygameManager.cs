using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//public class EarlygameManager : MonoBehaviour
//{
//    public Transform motherboardShelf;
//    public Transform cpuShelf;
//    public Transform ramShelf;
//    public Transform cashCounter;

//    private Queue<Transform> queue = new Queue<Transform>();

//    void OnTriggerEnter(Collider other)
//    {
//        // Check if the player entered the Cash counter trigger
//        if (other.CompareTag("AINPC"))
//        {
//            StartCoroutine(CheckOut());
//        }
//    }

//    IEnumerator CheckOut()
//    {
//        // Continue as long as there are customers in line
//        while (queue.Count > 0)
//        {
//            // Dequeue the next customer
//            Transform currentCustomer = queue.Dequeue();

//            // Simulate checking out
//            Debug.Log(currentCustomer.name + " is checking out.");

//            // Wait for a short time before processing the next customer
//            yield return new WaitForSeconds(1f);
//        }
//    }

//    void OnTriggerStay(Collider other)
//    {
//        // Check if an AI NPC entered the shelf trigger
//        if (other.CompareTag("AINPC"))
//        {
//            AINPC aiNPC = other.GetComponent<AINPC>();

//            // Add the AI NPC to the queue
//            if (!queue.Contains(other.transform))
//            {
//                queue.Enqueue(other.transform);
//            }

//            // Stop the AI NPC for 4 seconds to take the product
//            StartCoroutine(aiNPC.TakeProduct());
//        }
//    }
//}

