using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MoneyDeduction : MonoBehaviour
{
    public Shelf.ShelfType shelfTypeToSet = Shelf.ShelfType.CPU;
    public GameObject floorUI;
    public GameObject WorkerAI1, WorkerAI2;
    public GameObject activatePrefab;
    public GameObject UpgradeScreen;
    public GameObject WorkerStats;
    public GameObject CashierStats;
    public GameObject WorkerUnlock;
    public GameObject CashierUnlock;
    public int WorkerCost = 1200;
    public int CashierCost = 1200;
    public UnityEngine.UI.Image fillImage;
    public int totalDeductionAmount = 100; // Total amount of money to deduct when player is in range
    public GameObject cashPrefab; // Prefab of the cash object to spawn
    public Transform playerTransform; // Player's transform assigned in the inspector
    public TextMeshProUGUI floorText;
    public float delayBetweenJumps = 0.1f; // Delay between each jump
    private int remainingDeductionAmount; // Remaining amount to deduct
    private bool playerInRange = false; // Flag to track if player is in range
    private Coroutine deductionCoroutine; // Coroutine reference for deduction
    public WallAnimation wallAnimation;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(gameObject.CompareTag("PurchaseUI")){
                playerInRange = true;
                StartDeductionCoroutine();
            }
            if(gameObject.CompareTag("UpgradeUI")){
                UpgradeScreen.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(gameObject.CompareTag("PurchaseUI")){
                playerInRange = false;
                StopDeductionCoroutine();
            }
            if(gameObject.CompareTag("UpgradeUI")){
                UpgradeScreen.SetActive(false);
            }
        }
    }

    private void StartDeductionCoroutine()
    {
        if (deductionCoroutine == null)
        {
            deductionCoroutine = StartCoroutine(DeductMoneyAndSpawnCashCoroutine());
        }
    }

    private void StopDeductionCoroutine()
    {
        if (deductionCoroutine != null)
        {
            StopCoroutine(deductionCoroutine);
            deductionCoroutine = null;
        }
    }

    private IEnumerator DeductMoneyAndSpawnCashCoroutine()
{
    // Calculate the amount to deduct in each iteration
    float deductionPerIteration = totalDeductionAmount * Time.deltaTime / 10f; // Adjust the divisor to control the deduction rate

    while (playerInRange && remainingDeductionAmount > 0 && PlayerCashCounter.instance.totalCashValue > 0)
    {
        // Calculate the cash value to deduct for this iteration
        int cashValue = Mathf.Min((int)deductionPerIteration, remainingDeductionAmount, PlayerCashCounter.instance.totalCashValue);

        // Spawn the cash prefab
        GameObject cashInstance = Instantiate(cashPrefab, playerTransform.position, Quaternion.identity);

        // Deduct the cash value from the remaining deduction amount and player's cash
        remainingDeductionAmount -= cashValue;
        PlayerCashCounter.instance.DeductTotalCash(cashValue);

        // Move the cash object towards the player's position with a random jump
        float randomJumpHeight = Random.Range(2f, 4f);
            float randomJumpDuration = Random.Range(.1f, .6f);

            // Use DOTween to move the cash object towards the destination with random jump parameters
            cashInstance.transform.DOJump(transform.position, randomJumpHeight, 1, randomJumpDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => Destroy(cashInstance));

        // Update UI elements
        UpdateFloorUI();
        UpdateFillAmount();

        // Wait for the next frame
        yield return null;
    }

    if (remainingDeductionAmount <= 0)
        {
            // Disable the floorUI
            floorUI.SetActive(false);
            if(wallAnimation!=null){
                wallAnimation.AnimateShutter(wallAnimation.Shutter);
            }
             if(activatePrefab!=null){
                    // Enable the parent GameObject
                activatePrefab.SetActive(true);
                // Iterate through all children of the parent GameObject and enable them
                foreach (Transform child in activatePrefab.transform)
                {
                    child.gameObject.SetActive(true);
                }


                // Play popping animation on the parent GameObject
                activatePrefab.transform.localScale = Vector3.zero;
                activatePrefab.transform.DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => Debug.Log("Popping animation complete."));
            }
        }

    // Reset the coroutine reference
    deductionCoroutine = null;
}



    private void UpdateFloorUI()
    {
        if (floorText != null)
        {
            floorText.text = remainingDeductionAmount.ToString();
        }
    }

    private void UpdateFillAmount()
{
    if (fillImage != null)
    {
        // Calculate the fill amount based on the deduction progress
        float fillAmount = Mathf.Clamp01((float)(totalDeductionAmount - remainingDeductionAmount) / totalDeductionAmount);

        // Start the smooth fill animation coroutine
        StartCoroutine(SmoothFillAnimation(fillAmount, 0.5f)); // Adjust duration as needed for smoother animation
    }
}
private IEnumerator SmoothFillAnimation(float targetFillAmount, float duration)
{
    float currentFillAmount = fillImage.fillAmount;
    float timer = 0f;

    while (timer < duration)
    {
        timer += Time.deltaTime;
        float fillProgress = Mathf.Lerp(currentFillAmount, targetFillAmount, timer / duration);
        fillImage.fillAmount = fillProgress;
        yield return null;
    }

    // Ensure that the fill amount is exactly the target amount at the end of the animation
    fillImage.fillAmount = targetFillAmount;
}

    public void BuyWorker()
    {
        if (PlayerCashCounter.instance.totalCashValue >= WorkerCost)
        {
            PlayerCashCounter.instance.totalCashValue -= WorkerCost;
            if(WorkerAI1!=null){
                WorkerAI1.SetActive(true);
                PlayerCashCounter.instance.UpdateMoneyUI();
            }
            if(WorkerAI2!=null){
                WorkerAI2.SetActive(true);
                PlayerCashCounter.instance.UpdateMoneyUI();
            }
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
