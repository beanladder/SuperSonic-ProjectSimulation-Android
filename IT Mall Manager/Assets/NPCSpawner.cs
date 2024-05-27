using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject npcPrefab;
    public float spawnInterval = 8f;
    public int maxNPC = 40; // Maximum number of NPCs allowed

    public int currentNPCs = 0; // Current number of spawned NPCs

    private void Start()
    {
        InvokeRepeating("SpawnNPC", 0f, spawnInterval);
    }

    private void SpawnNPC()
    {
        if (currentNPCs < maxNPC)
        {
            // Choose a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Spawn the NPC at the chosen spawn point
            GameObject npc = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);

            // Get the AINPC component and set its spawner reference
            AINPC aiNPC = npc.GetComponent<AINPC>();
            aiNPC.SetSpawner(this);

            currentNPCs++; // Increment the count of spawned NPCs
        }
    }

    // Method to decrease the count of spawned NPCs when one gets destroyed
    public void NPCDestroyed()
    {
        currentNPCs--;
    }
}
