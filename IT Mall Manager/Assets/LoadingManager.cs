using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public string sceneToLoad = "Game"; // Name of the scene to load
    public Slider progressBar; // UI Slider to show loading progress
    public Text progressText; // Optional: Text to show percentage
    public float artificialLoadDuration = 3f; // Duration to simulate loading (in seconds)
    public float artificialIncrement = 0.02f; // Increment for artificial delay progress

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // Start loading the scene asynchronously
        Debug.Log("Starting to load scene: " + sceneToLoad);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        float simulatedProgress = 0f;
        float elapsedTime = 0f;

        while (!asyncOperation.isDone)
        {
            // Calculate simulated progress
            elapsedTime += Time.deltaTime;
            simulatedProgress = Mathf.Clamp01(elapsedTime / artificialLoadDuration);

            // Update progress bar and text
            if (progressBar != null)
            {
                progressBar.value = simulatedProgress;
            }

            if (progressText != null)
            {
                progressText.text = Mathf.RoundToInt(simulatedProgress * 100f) + "%";
            }

            Debug.Log("Loading progress: " + (simulatedProgress * 100f) + "%");

            // Artificial delay to make the progress smoother
            if (simulatedProgress < 1f)
            {
                yield return new WaitForSeconds(artificialIncrement);
            }
            else
            {
                yield return null;
            }
        }

        Debug.Log("Scene loaded and activated.");
    }
}
