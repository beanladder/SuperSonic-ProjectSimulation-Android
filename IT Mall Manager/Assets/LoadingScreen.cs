using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider progressBar;
    public float delayAfterLoad = 2.0f;
    private AsyncOperation loadOperation;

    private float currentValue = 0f;
    private float targetValue = 0f;
    private float progressAnimationMultiplier = 0.1f;

    void Start()
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // Start loading the scene
        loadOperation = SceneManager.LoadSceneAsync("Game");
        loadOperation.allowSceneActivation = false;

        // Update the progress bar
        while (!loadOperation.isDone)
        {
            // Update progress until 90%
            targetValue = loadOperation.progress / 0.9f;

            // Interpolate current value towards target value
            while (currentValue < targetValue)
            {
                currentValue = Mathf.MoveTowards(currentValue, targetValue, progressAnimationMultiplier * Time.deltaTime);
                progressBar.value = currentValue;
                yield return null;
            }

            // Check if the scene is almost done loading
            if (loadOperation.progress >= 0.9f)
            {
                break;
            }
        }

        // Set the progress bar to 100%
        targetValue = 1f;
        while (currentValue < targetValue)
        {
            currentValue = Mathf.MoveTowards(currentValue, targetValue, progressAnimationMultiplier * Time.deltaTime);
            progressBar.value = currentValue;
            yield return null;
        }

        // Wait for the delay
        yield return new WaitForSeconds(delayAfterLoad);

        // Allow the scene to be activated
        loadOperation.allowSceneActivation = true;
    }
}
