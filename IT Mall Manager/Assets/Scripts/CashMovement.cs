using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CashMovement : MonoBehaviour
{
    public Transform playerTransform; // Assign this to your player's transform in the inspector
    public float moveDuration = 1f; // Duration of the movement animation
    public float delayBetweenMovement = 0.1f; // Delay between each cash movement
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

        // Collect all cash objects from all spawn points
        List<List<GameObject>> allCashObjects = new List<List<GameObject>>();
        foreach (Transform child in transform)
        {
            if (child.name == "SpawnPoint")
            {
                List<GameObject> cashObjects = new List<GameObject>();
                foreach (Transform cash in child)
                {
                    cashObjects.Add(cash.gameObject);
                }
                allCashObjects.Add(cashObjects);
            }
        }

        bool hasMoreCash = true;
        while (hasMoreCash)
        {
            hasMoreCash = false;
            foreach (var cashObjects in allCashObjects)
            {
                if (cashObjects.Count > 0)
                {
                    hasMoreCash = true;
                    GameObject cashObject = cashObjects[0];
                    cashObjects.RemoveAt(0);

                    if (cashObject != null)
                    {
                        cashObject.transform.DOJump(playerTransform.position, 3f, 1, moveDuration)
                            .SetEase(Ease.InQuad)
                            .OnComplete(() =>
                            {
                                PlayerCashCounter.instance.IncreaseTotalCashReached(cashValuePerPrefab);
                                Destroy(cashObject);

                                // Add haptic feedback when cash reaches its destination
                                if (Application.platform == RuntimePlatform.Android)
                                {
                                    Handheld.Vibrate();
                                }
                            });
                    }

                    // Wait for the specified delay before moving the next cash object
                    yield return new WaitForSeconds(delayBetweenMovement);
                }
            }
        }

        isMovingCash = false;
    }
}
