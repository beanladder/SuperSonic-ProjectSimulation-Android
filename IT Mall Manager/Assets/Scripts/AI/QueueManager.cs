using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager instance;
    public static QueueManager Instance { get { return instance; } }

    private Queue<Transform> queue = new Queue<Transform>();

    void Awake()
    {
        // Singleton pattern to ensure there's only one instance of QueueManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddToQueue(Transform npcTransform)
    {
        queue.Enqueue(npcTransform);
        UpdateQueuePositions();
    }

    public Transform GetNextQueuePosition()
    {
        if (queue.Count > 0)
        {
            return queue.Peek(); // Return the transform of the next NPC in the queue
        }
        return transform; // Default to queue manager transform if no NPCs in queue
    }

    public void RemoveFromQueue()
    {
        if (queue.Count > 0)
        {
            queue.Dequeue();
            UpdateQueuePositions();
        }
    }

    private void UpdateQueuePositions()
    {
        int positionIndex = 1;
        foreach (Transform npcTransform in queue)
        {
            npcTransform.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(GetQueuePosition(positionIndex));
            positionIndex++;
        }
    }

    private Vector3 GetQueuePosition(int positionIndex)
    {
        // Define custom positions for the queue (adjust as needed for your scene)
        Vector3[] customPositions = new Vector3[]
        {
        new Vector3(0f, 0f, 0f),   // Position for index 1
        new Vector3(0f, 0f, 2f),   // Position for index 2
        new Vector3(0f, 0f, 4f),   // Position for index 3
                                   // Add more custom positions for higher indices as needed
        };

        // Check if the positionIndex is within the bounds of customPositions
        if (positionIndex > 0 && positionIndex <= customPositions.Length)
        {
            return transform.position + customPositions[positionIndex - 1]; // Adjust index to match array index
        }
        else
        {
            Debug.LogWarning("Position index out of bounds for custom queue positions.");
            // Default to a position based on the position index (adjust as needed)
            return transform.position + Vector3.right * positionIndex;
        }
    }
}
