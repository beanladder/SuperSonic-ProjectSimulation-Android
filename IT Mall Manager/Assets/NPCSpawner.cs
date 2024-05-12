using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject npcPrefab;
    public float spawnInterval = 8f;

    private void Start()
    {
        InvokeRepeating("SpawnNPC", 0f, spawnInterval);
    }

    private void SpawnNPC()
    {
        // Choose a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Spawn the NPC at the chosen spawn point
        Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
    }
}
