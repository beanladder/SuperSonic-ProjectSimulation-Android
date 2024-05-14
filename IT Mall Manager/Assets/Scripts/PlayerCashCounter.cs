using UnityEngine;

public class PlayerCashCounter : MonoBehaviour
{
    public int totalCashValue = 0; // Total cash value reached by the player
    
    public static PlayerCashCounter instance;

    private void Awake()
    {
        instance = this;
    }
    public void IncreaseTotalCashReached(int amount)
    {
        totalCashValue += amount; // Increase total cash value based on the amount and value per prefab
       
    }

    
    public void DeductTotalCash(int amount)
    {
        // Check if there's enough cash to deduct
        if (totalCashValue >= amount)
        {
            totalCashValue -= amount;
            
            
        }
        else
        {
            
        }
    }

    
}
