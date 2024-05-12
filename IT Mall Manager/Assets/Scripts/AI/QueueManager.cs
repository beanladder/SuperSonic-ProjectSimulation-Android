using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager instance;

    // List to hold the queue of AI NPCs
    public List<Transform> queue = new List<Transform>();

    private Dictionary<Transform, Transform> npcQueuePositions = new Dictionary<Transform, Transform>();

    // Array to hold the positions of the queue gameobjects
    public List<Transform> queuePositions = new List<Transform>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Add NPC to the queue
    public void AddToQueue(Transform npcTransform)
    {
        queue.Add(npcTransform);
        UpdateQueuePositions();
    }

    // Remove NPC from the queue (after checkout)
    public void RemoveFromQueue(Transform npcTransform)
    {
        if (queue.Contains(npcTransform))
        {
            queue.Remove(npcTransform);
            UpdateQueuePositions();
            //UpdateNavMeshDestinations(); // Update NavMeshAgent destinations after queue position changes
        }
    }

    // Update the positions of NPCs in the queue
    private void UpdateQueuePositions()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            // Ensure there are enough queue positions
            if (i < queuePositions.Count)
            {
                queue[i].position = queuePositions[i].position;
            }
            else
            {
                Debug.LogWarning("Not enough queue positions defined.");
                break;
            }
        }
    }

    // Update the NavMeshAgent destinations for all NPCs in the queue
    private void UpdateNavMeshDestinations()
    {
        foreach (Transform npcTransform in queue)
        {
            AINPC aiNPC = npcTransform.GetComponent<AINPC>();
            if (aiNPC != null)
            {
                aiNPC.MoveToCheckout(); // Call a method in AINPC to update the NavMeshAgent destination
            }
        }
    }

    // Get the first empty position in the queue
    public Transform GetFirstEmptyPosition()
    {
        foreach (Transform position in queuePositions)
        {
            bool isPositionEmpty = true;
            foreach (Transform npcTransform in queue)
            {
                if (npcTransform.position == position.position)
                {
                    isPositionEmpty = false;
                    break;
                }
            }
            if (isPositionEmpty)
                return position;
        }
        return null; // No empty position found
    }

    public Transform GetNextEmptyPosition(Transform npcTransform)
    {
        // Check if the NPC's transform already exists as a key in the dictionary
        if (!npcQueuePositions.ContainsKey(npcTransform))
        {
            foreach (Transform position in queuePositions)
            {
                bool isPositionEmpty = true;

                // Check if the position is already occupied by another NPC
                foreach (Transform npc in npcQueuePositions.Values)
                {
                    if (npc.position == position.position)
                    {
                        isPositionEmpty = false;
                        break;
                    }
                }

                if (isPositionEmpty)
                {
                    // Assign the empty position to the NPC
                    npcQueuePositions.Add(npcTransform, position); // Use the NPC's transform as the key
                    return position;
                }
            }
        }
        return null; // No empty position found or position already assigned for NPC
    }
    //private void UpdateQueuePositions()
    //{
    //    // Iterate through the queue and update NPC positions
    //    for (int i = 0; i < nextPositionIndex; i++)
    //    {
    //        // Set destination for NPCs at positions in the queue
    //        if (i < queuePositions.Count)
    //        {
    //            foreach (GameObject npc in GameObject.FindGameObjectsWithTag("AINPC"))
    //            {
    //                npc.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(queuePositions[i]);
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Not enough queue positions for all NPCs in the queue.");
    //            break;
    //        }
    //    }
    //}
}
