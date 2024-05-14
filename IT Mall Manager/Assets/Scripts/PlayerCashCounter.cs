using UnityEngine;
using UnityEngine.UI;

public class PlayerCashCounter : MonoBehaviour
{
    public int totalCashValue = 0; // Total cash value reached by the player
    public Text moneyText;
    public static PlayerCashCounter instance;

    private void Awake()
    {
        instance = this;
    }
    public void IncreaseTotalCashReached(int amount)
    {
        totalCashValue += amount; // Increase total cash value based on the amount and value per prefab
        UpdateMoneyUI();
    }

    
    public void DeductTotalCash(int amount)
    {
        // Check if there's enough cash to deduct
        if (totalCashValue >= amount)
        {
            totalCashValue -= amount;
            UpdateMoneyUI();
        }
        else
        {
            
        }
    }

    public void UpdateMoneyUI(){
        if(moneyText!=null){
            moneyText.text = totalCashValue.ToString();
        }
    }
}
