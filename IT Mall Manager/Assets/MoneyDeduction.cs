using DG.Tweening;
using UnityEngine;
using System.Collections;

public class MoneyDeduction : MonoBehaviour
{
    public int totalDeductionAmount = 100; // Total amount of money to deduct when player is in range
    public GameObject cashPrefab; // Prefab of the cash object to spawn
    public Transform playerTransform; // Player's transform assigned in the inspector

    public float jumpHeight = 2f; // Height of the jump
    public float jumpDuration = 0.4f; // Duration of each jump
    public float delayBetweenJumps = 0.1f; // Delay between each jump
    private int remainingDeductionAmount; // Remaining amount to deduct
    private bool playerInRange = false; // Flag to track if player is in range
    private Coroutine deductionCoroutine; // Coroutine reference for deduction

    private void Start()
    {
        remainingDeductionAmount = totalDeductionAmount;
        playerTransform = GetPostion.instance.playerTransform;
    }

    private void Update()
    {
        if (GetPostion.instance != null)
        {
            playerTransform = GetPostion.instance.playerTransform;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            StartDeductionCoroutine();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StopDeductionCoroutine();
        }
    }

    void StartDeductionCoroutine()
    {
        if (deductionCoroutine == null)
        {
            deductionCoroutine = StartCoroutine(DeductMoneyAndSpawnCashCoroutine());
        }
    }

    void StopDeductionCoroutine()
    {
        if (deductionCoroutine != null)
        {
            StopCoroutine(deductionCoroutine);
            deductionCoroutine = null;
        }
    }

    IEnumerator DeductMoneyAndSpawnCashCoroutine()
    {
        while (playerInRange && PlayerCashCounter.instance.totalCashValue >= remainingDeductionAmount)
        {
            Debug.Log("Deducted " + totalDeductionAmount + " from player's cash.");

            // Calculate the number of cash objects to spawn based on the remaining deduction amount
            int numberOfCash = remainingDeductionAmount / 500; // Each cash prefab is valued at 500

            for (int i = 0; i < numberOfCash; i++)
            {
                // Adjust spawn position based on the current player position
                Vector3 spawnPosition = playerTransform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

                // Spawn the cash prefab
                GameObject cashInstance = Instantiate(cashPrefab, spawnPosition, Quaternion.identity);

                // Deduct a fraction of the total amount for each cash object
                PlayerCashCounter.instance.DeductTotalCash(500);

                // Use DOTween to move the cash object towards the destination
                cashInstance.transform.DOJump(transform.position, jumpHeight, 1, jumpDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(cashInstance));

                // Calculate delay for the current cash object
                yield return new WaitForSeconds(delayBetweenJumps);
            }

            // Reset the remaining deduction amount
            remainingDeductionAmount = 0;
        }

        // Reset the coroutine reference
        deductionCoroutine = null;
    }
}
