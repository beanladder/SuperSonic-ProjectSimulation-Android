using TMPro;
using UnityEngine;

public class PoppingAnimation : MonoBehaviour
{
    // The duration of the pop in and pop out animations
    public float popDuration = 0.5f;

    // The scale factor for the pop in animation
    public float popScale = 1.2f;
    private bool isActive = false;
    private Canvas canvas;
    private Vector3 originalScale;

    void Awake()
    {
        // Cache the canvas component and original scale
        canvas = GetComponent<Canvas>();
        originalScale = transform.localScale;

        // Ensure the canvas starts hidden but active
        HideCanvas();
    }

    private void HideCanvas()
    {
        transform.localScale = Vector3.zero;
    }

    private void ShowCanvas()
    {
        transform.localScale = originalScale;
    }

    public void PopIn()
    {
        // Activate the canvas and start the pop in animation
        ShowCanvas();
        LeanTween.scale(gameObject, originalScale * popScale, popDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() =>
            {
                LeanTween.scale(gameObject, originalScale, popDuration * 0.5f)
                    .setEase(LeanTweenType.easeOutBack)
                    .setOnComplete(() => { isActive = true; });
            });
    }

    public void PopOut()
    {
        // Start the pop out animation
        LeanTween.scale(gameObject, Vector3.zero, popDuration)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                // Hide the canvas after the animation completes
                HideCanvas();
                isActive = false;
            });
    }

    public void TogglePop()
    {
        if (!isActive)
        {
            PopIn();
        }
        else
        {
            PopOut();
        }
    }

    public void SmileIn()
    {
        // Activate the canvas and start the pop in animation
        ShowCanvas();
        LeanTween.scale(gameObject, originalScale * popScale, popDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() =>
            {
                LeanTween.scale(gameObject, originalScale, popDuration * 0.5f)
                    .setEase(LeanTweenType.easeOutBack)
                    .setOnComplete(() => { isActive = true; });
            });
    }

    public void SmileOut()
    {
        // Start the pop out animation
        LeanTween.scale(gameObject, Vector3.zero, popDuration)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                // Hide the canvas after the animation completes
                HideCanvas();
                isActive = false;
            });
    }

    public void ToggleSmile()
    {
        if (!isActive)
        {
            SmileIn();
        }
        else
        {
            SmileOut();
        }
    }
}
