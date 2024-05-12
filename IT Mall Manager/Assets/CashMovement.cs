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

    private bool isInRange = false;

    void Update()
    {
        if (isInRange)
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
                    cashObject.transform.DOJump(playerTransform.position, 1f, 1, moveDuration).SetEase(Ease.Linear); // Adjust jump height (1f) and number of jumps (1) as needed
                    yield return new WaitForSeconds(delayBetweenMovement); // Introduce delay between moving each cash object
                    Destroy(cashObject);
                }
            }

        }
    }
}
