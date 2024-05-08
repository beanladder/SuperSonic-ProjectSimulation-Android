using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutManager : MonoBehaviour
{
    private Queue<Transform> checkoutQueue = new Queue<Transform>();

    void OnTriggerEnter(Collider other)
    {
        // Check if an AI NPC entered the checkout trigger
        if (other.CompareTag("AINPC"))
        {
            // Add the AI NPC to the checkout queue
            checkoutQueue.Enqueue(other.transform);

            // Reorder the queue to form a straight line
            ReorderQueue();
        }
    }

    void ReorderQueue()
    {
        // Create a list to store the ordered AI NPCs
        List<Transform> orderedQueue = new List<Transform>();

        // Add the AI NPCs from the queue to the list in order
        while (checkoutQueue.Count > 0)
        {
            orderedQueue.Add(checkoutQueue.Dequeue());
        }

        // Clear the original queue
        checkoutQueue.Clear();

        // Add the AI NPCs back to the queue in the correct order
        foreach (Transform aiNPC in orderedQueue)
        {
            checkoutQueue.Enqueue(aiNPC);
        }
    }
}
