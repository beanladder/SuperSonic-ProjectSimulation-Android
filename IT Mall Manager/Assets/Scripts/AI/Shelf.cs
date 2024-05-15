using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public static Shelf instance;

    // Enum to represent the different types of shelves
    public enum ShelfType { CPU, RAM, Motherboard }
    public int productCount;

    // Variable to hold the type of this shelf
    public ShelfType shelfType;

    // Array to hold references to the spawn points
    public Transform[] spawnPoints;

    // Prefabs for RAM, CPU, and Motherboard
    public GameObject ramPrefab;
    public GameObject cpuPrefab;
    public GameObject motherboardPrefab;

    private void Awake()
    {
        instance = this;

        // Find all spawn points that are children of this shelf (excluding the shelf itself)
        List<Transform> childTransforms = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child != transform) // Exclude the parent shelf object
            {
                childTransforms.Add(child);
            }
        }
        spawnPoints = childTransforms.ToArray();
    }

    // Detect collision with the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided with shelf of type: " + shelfType);
        }
    }
}
