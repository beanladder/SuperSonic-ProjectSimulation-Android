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

        // Ensure the canvas starts inactive
        canvas.gameObject.SetActive(false);
    }

    public void PopIn()
    {
        // Activate the canvas and start the pop in animation
        canvas.gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
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
                // Deactivate the canvas after the animation completes
                canvas.gameObject.SetActive(false);
                // Reset scale to original for the next pop in
                transform.localScale = originalScale;
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
}
