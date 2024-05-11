using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CashOutflow : MonoBehaviour
{
    [SerializeField] Transform[] CashHolders = new Transform[6]; // Array to hold the 6 empty GameObject transforms
    [SerializeField] GameObject CashPrefab; // Prefab of the cash
    [SerializeField] float CashDeliveryTime, Yaxis; // Time between each cash spawn and y-axis offset

    bool playerNearby = false;
    Vector3 playerPosition;

    void Start()
    {
        // No need to find child transforms, assign them in the Inspector directly
        StartCoroutine(CashSpawn(CashDeliveryTime));
    }

    void Update()
    {
        if (playerNearby)
        {
            // Update player's position continuously
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            playerPosition = other.transform.position; // Set initial player position
            StartCoroutine(MoveCashTowardsPlayer()); // Start moving cash towards player
            Destroy(CashPrefab);
        }
    }

    IEnumerator MoveCashTowardsPlayer()
    {
        while (playerNearby)
        {
            // Move the cash towards the player with an animation
            for (int i = transform.childCount-1; i >=0; i--)
            {
                Transform cash = transform.GetChild(i);
                cash.DOJump(playerPosition, 2f, 1, 0.5f).SetEase(Ease.OutQuad); // Move towards the player's current position
                yield return new WaitForSeconds(0.1f); // Delay between moving each cash object
            }
            yield return null; // Wait for the next frame to update player's position
        }
    }

    

    public IEnumerator CashSpawn(float time)
    {
        int cashIndex = 0; // Index to track the current cash holder
        int countCash = 0; // Counter to track the total number of cash spawned
        while (countCash < 100) // You can adjust the condition as needed
        {
            GameObject newCash = Instantiate(CashPrefab, transform.position, Quaternion.identity, transform); // Instantiate cash at the dispenser position

            // Move the cash to the respective cash holder with a jump animation
            newCash.transform.DOJump(new Vector3(CashHolders[cashIndex].position.x, CashHolders[cashIndex].position.y + Yaxis, CashHolders[cashIndex].position.z), 2f, 1, 0.5f).SetEase(Ease.OutQuad);

            // Increment cash index and adjust y-axis offset if needed
            cashIndex = (cashIndex + 1) % CashHolders.Length; // Wrap around to the beginning if index exceeds the array length
            if (cashIndex == 0) // Increase Y-axis offset if a full cycle of cash holders is completed
            {
                Yaxis += 0.05f; // You can adjust this value as needed
            }

            countCash++; // Increment total cash count
            yield return new WaitForSeconds(time); // Wait for the specified time before spawning the next cash
        }
    }
}
