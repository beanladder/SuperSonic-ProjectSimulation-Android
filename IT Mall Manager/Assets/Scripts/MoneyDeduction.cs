using DG.Tweening;
using UnityEngine;
using System.Collections;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
public class MoneyDeduction : MonoBehaviour
{
    public static MoneyDeduction instance;
    public Image fillImage;
    public int totalDeductionAmount = 100; // Total amount of money to deduct when player is in range
    public GameObject cashPrefab; // Prefab of the cash object to spawn
    public Transform playerTransform; // Player's transform assigned in the inspector
    public TextMeshProUGUI floorText;
    public float jumpHeight = 2f; // Height of the jump
    public float jumpDuration = 0.4f; // Duration of each jump
    public float delayBetweenJumps = 0.1f; // Delay between each jump
    public int remainingDeductionAmount; // Remaining amount to deduct
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
        while (playerInRange && remainingDeductionAmount > 0 && PlayerCashCounter.instance.totalCashValue>0)
        {
            Debug.Log("Deducting " + totalDeductionAmount + " from player's cash.");

            // Calculate the number of cash objects to spawn based on the remaining deduction amount
            int cashValue = Mathf.Min(remainingDeductionAmount, 500); // Calculate the cash value to deduct
            int numberOfCash = cashValue / 500; // Each cash prefab is valued at 500

            for (int i = 0; i < numberOfCash; i++)
            {
                // Adjust spawn position based on the current player position
                Vector3 spawnPosition = playerTransform.position;

                // Spawn the cash prefab
                GameObject cashInstance = Instantiate(cashPrefab, spawnPosition, Quaternion.identity);

                // Deduct the cash value from the remaining deduction amount
                remainingDeductionAmount -= cashValue;
                PlayerCashCounter.instance.DeductTotalCash(500);

                float randomJumpHeight = Random.Range(1f, 2f);
                float randomJumpDuration = Random.Range(.1f, .6f);

                // Use DOTween to move the cash object towards the destination with random jump parameters
                cashInstance.transform.DOJump(transform.position, randomJumpHeight, 1, randomJumpDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => Destroy(cashInstance));
                

                // Calculate delay for the current cash object
                yield return new WaitForSeconds(delayBetweenJumps);
                UpdateFloorUI();
            }
        }
        if (remainingDeductionAmount <= 0)
        {
            // Destroy the GameObject to which this script is attached
            Destroy(gameObject);
        }

        // Reset the coroutine reference
        deductionCoroutine = null;
    }
    public void UpdateFloorUI(){
        if(floorText!=null){
            floorText.text = remainingDeductionAmount.ToString();
        }
    }
}