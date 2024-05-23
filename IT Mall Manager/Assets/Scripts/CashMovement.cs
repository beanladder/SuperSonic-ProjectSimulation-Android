using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CashMovement : MonoBehaviour
{
    public Transform playerTransform; // Assign this to your player's transform in the inspector
    public float moveDuration = 1f; // Duration of the movement animation
    public float delayBetweenMovement = 0.1f; // Delay between each spawn point's movement
    public int cashValuePerPrefab = 100; // Value to increase per cash prefab

    private bool isMovingCash = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isMovingCash)
        {
            StartCoroutine(MoveCashToPlayer());
        }
    }

    private IEnumerator MoveCashToPlayer()
    {
        isMovingCash = true;

        // Find all child objects with the name "SpawnPoint"
        List<Transform> spawnPoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name == "SpawnPoint")
            {
                spawnPoints.Add(child);
            }
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            List<GameObject> cashObjects = new List<GameObject>();
            foreach (Transform child in spawnPoint)
            {
                cashObjects.Add(child.gameObject);
            }

            // Move all cash objects in the current spawn point simultaneously
            foreach (GameObject cashObject in cashObjects)
            {
                if (cashObject != null)
                {
                    cashObject.transform.DOJump(playerTransform.position, 4f, 1, moveDuration)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            PlayerCashCounter.instance.IncreaseTotalCashReached(cashValuePerPrefab);
                            Destroy(cashObject);
                        });
                }
            }

            // Wait for the specified delay before moving the next spawn point's cash objects
            yield return new WaitForSeconds(delayBetweenMovement);
        }

        isMovingCash = false;
    }
}
