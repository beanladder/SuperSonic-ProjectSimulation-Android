using UnityEngine;

public class BounceAndPopAnimation : MonoBehaviour
{
    // The amount by which the image will move up and down
    public float bounceHeight = 10f;

    // The duration of one up and down cycle
    public float bounceDuration = 2f;

    // The interval between pop in and pop out animations
    public float popInterval = 3f;

    // The duration of the pop in and pop out animations
    public float popDuration = 0.5f;

    // The scale factor for the pop in animation
    public float popScale = 1.2f;

    void Start()
    {
        // Start the bounce animation
        StartBouncing();

        // Start the pop in and pop out animations
        InvokeRepeating("StartPopping", popInterval, popInterval);
    }

    void StartBouncing()
    {
        // Get the initial position of the image
        Vector3 originalPosition = transform.localPosition;

        // Define the target position for the bounce (moving up)
        Vector3 targetPosition = originalPosition + new Vector3(0, bounceHeight, 0);

        // Animate the image moving up and then down in a loop
        LeanTween.moveLocalY(gameObject, targetPosition.y, bounceDuration / 2)
            .setEaseInOutSine()
            .setLoopPingPong();
    }

    void StartPopping()
    {
        // Pop in (scale up)
        LeanTween.scale(gameObject, Vector3.one * popScale, popDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                // Pop out (scale down) after a short delay
                LeanTween.scale(gameObject, Vector3.one, popDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setDelay(popInterval - popDuration);
            });
    }
}
