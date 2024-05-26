using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Animations;

public class MoneyDeduction : MonoBehaviour
{
    public Shelf.ShelfType shelfTypeToSet = Shelf.ShelfType.CPU;
    public GameObject floorUI;
    public GameObject activatePrefab;
    public GameObject UpgradeScreen;
    public GameObject WorkerStats;
    public GameObject CashierStats;
    public GameObject WorkerUnlock;
    public GameObject CashierUnlock;
    public int WorkerCost = 1200;
    public int CashierCost = 1200;
    public static MoneyDeduction instance;
    public UnityEngine.UI.Image fillImage;
    public int totalDeductionAmount = 100; // Total amount of money to deduct when player is in range
    public GameObject cashPrefab; // Prefab of the cash object to spawn
    public Transform playerTransform; // Player's transform assigned in the inspector
    public TextMeshProUGUI floorText;
    public float jumpHeight = 2f; // Height of the jump
    public float jumpDuration = 0.4f; // Duration of each jump
    public float delayBetweenJumps = 0.1f; // Delay between each jump
    public int remainingDeductionAmount; // Remaining amount to deduct
    public UpgradeCanvasAnimator upgradeCanvasAnimator;
    private bool playerInRange = false; // Flag to track if player is in range
    private Coroutine deductionCoroutine; // Coroutine reference for deduction
    private Coroutine uiactivisionCoroutine;
    private const string FillAmountKey = "FillAmount";
    private const string DeductionAmountKey = "DeductAmount";
    private const string FloorUiActiveKey = "FloorUIActive";
    private const string PoppingPrefabActiveKey = "PoppingPrefabActive";

    // Add a delay duration variable
    public float deductionStartDelay = 1.0f; // Delay before starting deduction in seconds

    private void Start()
    {
        // LoadState();
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
            UpdateFillAmountImmediate(); // Update fill image immediately on entering the trigger
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
            deductionCoroutine = StartCoroutine(DeductionStartDelayCoroutine());
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

    // Coroutine to handle the delay before starting the deduction
    IEnumerator DeductionStartDelayCoroutine()
    {
        yield return new WaitForSeconds(deductionStartDelay);
        if (playerInRange)
        {
            deductionCoroutine = StartCoroutine(DeductMoneyAndSpawnCashCoroutine());
        }
    }

    IEnumerator DeductMoneyAndSpawnCashCoroutine()
    {
        while (playerInRange && remainingDeductionAmount > 0 && PlayerCashCounter.instance.totalCashValue > 0)
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
                UpdateFloorUI();
                UpdateFillAmountImmediate(); // Update fill image immediately
                // SaveState();
                yield return new WaitForSeconds(delayBetweenJumps);
            }
        }
        if (remainingDeductionAmount <= 0)
        {
            // Activate the prefab with popping animation
            activatePrefab.SetActive(true);
            floorUI.SetActive(false);

            // Set initial scale to zero
            activatePrefab.transform.localScale = Vector3.zero;

            // Store the initial position
            Vector3 targetPosition = activatePrefab.transform.position;

            // Set initial position slightly below the target position
            activatePrefab.transform.position = new Vector3(targetPosition.x, targetPosition.y - 1f, targetPosition.z);

            // Tween the scale to pop up and move it to the target position
            Sequence popSequence = DOTween.Sequence();
            popSequence.Append(activatePrefab.transform.DOMoveY(targetPosition.y, 0.5f).SetEase(Ease.OutBack))
                       .Join(activatePrefab.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack))
                       .OnComplete(() => Debug.Log("Popping animation complete."));
        }

        // Reset the coroutine reference
        deductionCoroutine = null;
    }

    public void UpdateFloorUI()
    {
        if (floorText != null)
        {
            floorText.text = remainingDeductionAmount.ToString();
        }
    }

    // Update the fill amount immediately
    void UpdateFillAmountImmediate()
    {
        if (fillImage != null)
        {
            // Calculate the fill amount based on the deduction progress
            float fillAmount = Mathf.Clamp01((float)(totalDeductionAmount - remainingDeductionAmount) / totalDeductionAmount);
            fillImage.fillAmount = fillAmount;
        }
    }

    IEnumerator DelayedActivision()
    {
        yield return new WaitForSeconds(1.8f);
        UpgradeScreen.SetActive(true);
    }

    private void SaveState()
    {
        PlayerPrefs.SetFloat(FillAmountKey, fillImage.fillAmount);
        PlayerPrefs.SetInt(DeductionAmountKey, remainingDeductionAmount);
        PlayerPrefs.Save();
        Debug.Log("State Saved : Remaining deduction amount = " + remainingDeductionAmount);
    }

    private void LoadState()
    {
        float savedfillamount = PlayerPrefs.GetFloat(FillAmountKey, 0f);
        fillImage.fillAmount = savedfillamount;
        remainingDeductionAmount = PlayerPrefs.GetInt(DeductionAmountKey, totalDeductionAmount);
        UpdateFloorUI();
        Debug.Log("State loaded : Remaining Deduction amount : " + remainingDeductionAmount);
    }

    public void ResetPref()
    {
        PlayerPrefs.DeleteKey(DeductionAmountKey);
        PlayerPrefs.DeleteKey(FillAmountKey);
    }

    public void BuyWorker()
    {
        if (PlayerCashCounter.instance.totalCashValue >= WorkerCost)
        {
            PlayerCashCounter.instance.totalCashValue -= WorkerCost;
            WorkerUnlock.SetActive(false);
            WorkerStats.SetActive(true);
        }
    }

    public void BuyCashier()
    {
        if (PlayerCashCounter.instance.totalCashValue >= CashierCost)
        {
            PlayerCashCounter.instance.totalCashValue -= CashierCost;
            CashierUnlock.SetActive(false);
            CashierStats.SetActive(true);
        }
    }
}
