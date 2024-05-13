using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CashMovement : MonoBehaviour
{
    public Transform playerTransform;
    public Transform tableTransform;
    public float moveDuration = 1f;
    public float delayBetweenMovement = 0.0000000001f;
    public int cashReachedPlayer=0;
    private bool isInRange = false;
    private bool isMovingCash = false; // Flag to track whether cash movement coroutine is running

    void Update()
    {
        if (isInRange && !isMovingCash)
        {
            StartCoroutine(MoveCashTowardsPlayer());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    IEnumerator MoveCashTowardsPlayer()
    {
        isMovingCash = true; // Set the flag to true to indicate coroutine is running

        try
        {
            // Get all the spawn points of the table
            Transform[] spawnPoints = tableTransform.GetComponentsInChildren<Transform>();

            foreach (Transform spawnPoint in spawnPoints)
            {
                // Exclude the table's own transform
                if (spawnPoint == tableTransform)
                    continue;

                // Get all the cash objects in this spawn point
                List<GameObject> cashObjects = new List<GameObject>();
                foreach (Transform child in spawnPoint)
                {
                    cashObjects.Add(child.gameObject);
                }

                // Reverse the list of cash objects to move them from last to first
                cashObjects.Reverse();

                // Move each cash object towards the player one by one
                foreach (GameObject cashObject in cashObjects)
                {
                    if (cashObject != null)
                    {
                        cashObject.transform.DOJump(playerTransform.position, 1f, 1, moveDuration).SetEase(Ease.Linear)
                            .OnComplete(() => { CashReachedPlayer(); }); // Call CashReachedPlayer when cash object reaches the player
                        yield return new WaitForSeconds(delayBetweenMovement); // Introduce delay between moving each cash object
                        Destroy(cashObject);
                    }
                }
            }
        }
        finally
        {
            isMovingCash = false; // Reset the flag when coroutine completes or is interrupted
        }
    }

    void CashReachedPlayer()
    {
        cashReachedPlayer++;
        Debug.Log("Cash reached player.");
    }
}
