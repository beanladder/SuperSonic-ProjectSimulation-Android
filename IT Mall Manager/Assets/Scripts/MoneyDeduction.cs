using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
public class MoneyDeduction : MonoBehaviour
{
    public GameObject yourPoppingPrefab;
    public GameObject UpgradeScreen;
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
            if(gameObject.CompareTag("PurchaseUI")){
                playerInRange = true;
                StartDeductionCoroutine();
            }
            else if(gameObject.CompareTag("UpgradeUI")){
                StartCoroutine(DelayedActivision());
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
            else if(gameObject.CompareTag("UpgradeUI")){
                UpgradeScreen.SetActive(false);
            }
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
                UpdateFloorUI();
                UpdateFillAmount();
                yield return new WaitForSeconds(delayBetweenJumps);
                
            }
            
        }
        if (remainingDeductionAmount <= 0)
        {
            // Disable canvas
            gameObject.SetActive(false);

            // Spawn prefab with popping animation
            GameObject poppingPrefab = Instantiate(yourPoppingPrefab, transform.position, Quaternion.identity);
            // Apply popping animation using AnimationCurve or other tweening method
            // For example, you can use DOTween to scale the prefab up and down
            poppingPrefab.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutElastic);
        }

        // Reset the coroutine reference
        deductionCoroutine = null;
    }
    public void UpdateFloorUI(){
        if(floorText!=null){
            floorText.text = remainingDeductionAmount.ToString();
        }
    }
   void UpdateFillAmount()
    {
        if (fillImage != null)
        {
            // Calculate the fill amount based on the deduction progress
            float fillAmount = Mathf.Clamp01((float)(totalDeductionAmount - remainingDeductionAmount) / totalDeductionAmount);

            // Start the smooth fill animation coroutine
            StartCoroutine(SmoothFillAnimation(fillAmount, 0.5f)); // Adjust duration as needed for smoother animation
        }
    }

    IEnumerator SmoothFillAnimation(float targetFillAmount, float duration)
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

    IEnumerator DelayedActivision(){
        yield return new WaitForSeconds(1.0f);
        UpgradeScreen.SetActive(true);
    }
}
