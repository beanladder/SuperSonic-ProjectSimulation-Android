using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonHandler : MonoBehaviour
{
    public GameObject playerTab;
    public Image[] statBarImages; // Array of Image components for multiple stat bars
    public Color upgradedColor = Color.green; // The color to change to when upgraded
    private int currentUpgradeIndex = 0; // Keeps track of the current upgrade level

    // This function will be called when the button is clicked
    public void OnUpgradeSpeed()
    {
        if (currentUpgradeIndex < statBarImages.Length)
        {
            // Change the color of the current stat bar image to indicate an upgrade
            statBarImages[currentUpgradeIndex].color = upgradedColor;
            currentUpgradeIndex++;
        }
    }

    public void OnUpgradeCapacity()
    {
        if (currentUpgradeIndex < statBarImages.Length)
        {
            // Change the color of the current stat bar image to indicate an upgrade
            statBarImages[currentUpgradeIndex].color = upgradedColor;
            currentUpgradeIndex++;
        }
    }
    public void SwitchTab(){
        playerTab.SetActive(false);
    }
}
