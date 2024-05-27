using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingCanvas; // Reference to the loading canvas
    public Slider loadingSlider; // Reference to the loading slider (if any)
    public float loadingDuration = 3.0f; // Duration for which the loading screen will be displayed
    public float minSpeedMultiplier = 0.1f; // Minimum speed multiplier for slowdowns
    public float maxSpeedMultiplier = 2.0f; // Maximum speed multiplier for speedups
    public float changeInterval = 0.5f; // Interval at which the speed can change

    private void Start()
    {
        // Ensure the loading canvas is active at the start
        if (loadingCanvas != null)
        {
            loadingCanvas.SetActive(true);
        }

        // Start the loading process
        StartCoroutine(SimulateLoading());
    }

    private IEnumerator SimulateLoading()
    {
        float elapsedTime = 0f;
        float currentSpeed = 1f; // Current speed multiplier

        // Gradually fill the loading bar (if any) over the loading duration
        while (elapsedTime < loadingDuration)
        {
            elapsedTime += Time.deltaTime * currentSpeed;

            if (loadingSlider != null)
            {
                loadingSlider.value = Mathf.Clamp01(elapsedTime / loadingDuration);
            }

            // Randomly change the speed at regular intervals
            if (elapsedTime % changeInterval < Time.deltaTime)
            {
                currentSpeed = Random.Range(minSpeedMultiplier, maxSpeedMultiplier);
            }

            yield return null;
        }

        // Ensure the slider is full at the end of the loading duration
        if (loadingSlider != null)
        {
            loadingSlider.value = 1f;
        }

        // Hide the loading canvas
        if (loadingCanvas != null)
        {
            loadingCanvas.SetActive(false);
        }
    }
}
