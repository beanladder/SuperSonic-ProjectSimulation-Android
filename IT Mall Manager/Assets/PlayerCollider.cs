using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerCollider : MonoBehaviour
{
    private bool isInRange = false;
    public Transform table; // Reference to the table game object
    public float jumpHeight = 2f;
    public float duration = 0.01f;
    public int jumps = 1;
    public float delayBetweenCash = 0.1f; // Delay between moving each cash prefab

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == table)
        {
            isInRange = true;
            StartMovingCashTowardsPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == table)
        {
            isInRange = false;
            StopCashMovement();
        }
    }

    private void StartMovingCashTowardsPlayer()
    {
        StartCoroutine(MoveCashTowardsPlayerCoroutine());
    }

    private void StopCashMovement()
    {
        DOTween.Kill(transform); // Kill all tweens associated with the player transform
    }

    private IEnumerator MoveCashTowardsPlayerCoroutine()
    {
        while (isInRange)
        {
            Vector3 playerPosition = transform.position; // Update player position every frame
            foreach (Transform cashHolder in table)
            {
                foreach (Transform cashPrefab in cashHolder)
                {
                    // Move each cash prefab towards player with a delay
                    cashPrefab.DOJump(playerPosition, jumpHeight, jumps, duration).SetEase(Ease.OutQuad);
                    yield return new WaitForSeconds(delayBetweenCash);
                }
            }
            yield return null; // Wait for the next frame
        }
    }
}
