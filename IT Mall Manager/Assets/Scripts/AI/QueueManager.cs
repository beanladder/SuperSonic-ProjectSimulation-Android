using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager instance;
    public static QueueManager Instance { get { return instance; } }

    public List<Transform> queuePositions = new List<Transform>();
    public int nextPositionIndex = -1;

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

    public void AddToQueue()
    {
        // Ensure there are positions in the queue
        if (nextPositionIndex < queuePositions.Count)
        {
            nextPositionIndex++; // Increment the next available position index
            
        }
        else
        {
            Debug.LogWarning("No more positions available in the queue.");
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
}
