using UnityEngine;

public class PlayerCashCounter : MonoBehaviour
{
    public int totalCashValue = 0; // Total cash reached by the player
    private int cashValuePerPrefab = 500;
    public static PlayerCashCounter instance;

    private void Awake()
    {
        instance = this;
    }
    // Method to increase the total cash reached by the player
    public void IncreaseTotalCashReached(int amount)
    {
        totalCashValue += amount * cashValuePerPrefab;

        Debug.Log("Total cash reached by the player: " + totalCashValue);
        // You can add any additional functionality here, like updating UI or triggering events.
    }

    // Method to get the current total cash reached by the player
    public int GetTotalCashReached()
    {
        return totalCashValue;
    }
}
