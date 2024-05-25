using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QueueManager : MonoBehaviour
{
    public static Dictionary<string, QueueManager> instances = new Dictionary<string, QueueManager>(); // To hold instances of all QueueManagers

    public string storeName;
    public Transform[] queuePositions;

    private List<AINPC> queue = new List<AINPC>();

    private void Awake()
    {
        if (!instances.ContainsKey(storeName))
        {
            instances.Add(storeName, this);
        }
        else
        {
            Debug.LogError("QueueManager with store name " + storeName + " already exists!");
            Destroy(gameObject);
        }
    }

    // Add NPC to the queue
    public void AddToQueue(AINPC npc)
    {
        if (queue.Count >= queuePositions.Length)
        {
            // Queue is full, NPC should wait
            StartCoroutine(WaitForQueuePosition(npc));
        }
        else
        {
            queue.Add(npc);
            UpdateQueuePositions();
        }
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

    // Coroutine to wait for a queue position
    private IEnumerator WaitForQueuePosition(AINPC npc)
    {
        yield return new WaitForSeconds(15f); // Wait for 15 seconds
        if (queue.Count < queuePositions.Length)
        {
            // Queue position available, add NPC to the queue
            AddToQueue(npc);
        }
        else
        {
            // Queue still full after waiting, move NPC to random spawn point
            npc.MoveToRandomSpawnPoint();
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

