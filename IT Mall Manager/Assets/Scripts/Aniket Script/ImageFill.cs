using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFill : MonoBehaviour
{
    public Image timerImage; // Reference to the UI Image with fill feature
    public float fillDuration = 5f; // Duration it takes to fill the timer
    public string targetTag = "Player"; // Tag of the GameObject to trigger the timer
    public UpgradeCanvasAnimator upgradeCanvasAnimator; // Reference to the UpgradeCanvasAnimator
    private bool playerInRange = false;
    private Coroutine fillCoroutine = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log("Player entered the trigger zone.");
            playerInRange = true;
            timerImage.gameObject.SetActive(true);
            StartFilling();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log("Player exited the trigger zone.");
            playerInRange = false;
            StopFilling();
        }
    }

    void StartFilling()
    {
        if (fillCoroutine == null)
        {
            Debug.Log("Starting to fill the timer.");
            fillCoroutine = StartCoroutine(FillTimer());
        }
    }

    void StopFilling()
    {
        if (fillCoroutine != null)
        {
            Debug.Log("Stopping the fill coroutine.");
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }
        timerImage.fillAmount = 0f; // Reset the fill amount when out of range
    }

    IEnumerator FillTimer()
    {
        float elapsed = 0f;
        while (playerInRange && elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            timerImage.fillAmount = Mathf.Clamp01(elapsed / fillDuration);
            yield return null;
        }

        if (elapsed >= fillDuration)
        {
            // Timer is fully filled
            Debug.Log("Timer completed!");
            timerImage.gameObject.SetActive(false); // Disable the timer image
            if (upgradeCanvasAnimator != null)
            {
                upgradeCanvasAnimator.gameObject.SetActive(true); // Activate the upgrade canvas
            }
        }

        // Reset the coroutine reference
        fillCoroutine = null;
    }
}
