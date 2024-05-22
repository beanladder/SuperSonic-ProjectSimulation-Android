using UnityEngine;

public class UpgradeCanvasController : MonoBehaviour
{
    public RectTransform upgradeCanvas;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the upgrade canvas is initially disabled
        upgradeCanvas.gameObject.SetActive(false);
    }

    // Call this method to show the upgrade canvas
    public void ShowUpgradeCanvas()
    {
        // Enable the upgrade canvas
        upgradeCanvas.gameObject.SetActive(true);

        // Get the current position of the upgrade canvas
        Vector3 currentPosition = upgradeCanvas.localPosition;

        // Calculate the desired position (assuming canvas moves up by 500 units)
        Vector3 desiredPosition = currentPosition + new Vector3(0f, 500f, 0f);

        // Set the canvas below the screen
        upgradeCanvas.localPosition = currentPosition - new Vector3(0f, 500f, 0f);

        // Move the canvas to its desired position using LeanTween
        LeanTween.moveLocalY(upgradeCanvas.gameObject, desiredPosition.y, 0.5f)
            .setEase(LeanTweenType.easeOutBounce); // Adjust ease type as needed
    }
}
