using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager instance;

    // Array to hold the positions of the queue gameobjects
    public Transform[] queuePositions;

    private List<AINPC> queue = new List<AINPC>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Add NPC to the queue
    public void AddToQueue(AINPC npc)
    {
        queue.Add(npc);
        UpdateQueuePositions();
    }

    // Remove NPC from the queue (after checkout)
    public void RemoveFromQueue(AINPC npc)
    {
        if (queue.Contains(npc))
        {
            queue.Remove(npc);
            UpdateQueuePositions();
        }
    }

    // Update the positions of NPCs in the queue
    private void UpdateQueuePositions()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            queue[i].linePosition = i;
            queue[i].MoveToCheckout(queuePositions[i].position);
        }
    }
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

