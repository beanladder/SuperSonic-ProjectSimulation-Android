using UnityEngine;
using DG.Tweening;

public class UpgradeCanvasAnimator : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform canvasRectTransform; // Reference to the canvas's RectTransform
    public float fadeInDuration = 10f;
    public float moveDistance = 200f; // Adjust as needed
    public float scaleDuration = 3f; // Duration for scaling animation
    public Vector3 originalScale = Vector3.one; // Original scale of the canvas

    private Vector3 originalPosition;

    void Awake()
    {
        // Save the original position
        originalPosition = transform.localPosition;
    }

    void OnEnable()
    {
        PlayShowAnimation();
    }

    public void BackButton()
    {
        PlayHideAnimation();
    }

    private void PlayShowAnimation()
    {
        // Set initial alpha to 0
        canvasGroup.alpha = 0f;

        // Calculate the starting position further below
        Vector3 startPosition = originalPosition - Vector3.up * moveDistance;
        transform.localPosition = startPosition;

        // Set initial scale to only affect Y-axis
        canvasRectTransform.localScale = new Vector3(1f, 0f, 1f);

        // Fade in animation with movement and scaling from further below
        canvasGroup.DOFade(1f, fadeInDuration);
        transform.DOLocalMoveY(originalPosition.y, fadeInDuration).SetEase(Ease.OutCubic);
        canvasRectTransform.DOScaleY(originalScale.y, scaleDuration).SetEase(Ease.OutCubic).SetDelay(fadeInDuration - scaleDuration);
    }

    private void PlayHideAnimation()
{
    // Calculate the target position further below for hiding
    float targetPositionY = originalPosition.y - moveDistance;

    // Fade out animation with movement and scaling
    canvasGroup.DOFade(0f, fadeInDuration);
    transform.DOLocalMoveY(targetPositionY, fadeInDuration).SetEase(Ease.OutCubic);
    canvasRectTransform.DOScaleY(0f, scaleDuration).SetEase(Ease.OutCubic).OnComplete(() => gameObject.SetActive(false));
}

}
