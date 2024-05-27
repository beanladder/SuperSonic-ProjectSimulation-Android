using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCashCounter : MonoBehaviour
{
    public static PlayerCashCounter instance;
    public int totalCashValue = 0; // Total cash value reached by the player
    public TextMeshProUGUI moneyText;
    public GameObject ActivateExpansionShop;
    public GameObject ActivateMonopolyShop;
    private const string MoneyKey = "PlayerMoney";
    
    private bool expansionShopActivated = false;
    private bool monopolyShopActivated = false;

    private void Awake()
    {
        instance = this;
        //LoadMoney();
    }

    void Update()
    {
        if (totalCashValue >= 75000 && !expansionShopActivated)
        {
            if (ActivateExpansionShop != null)
            {
                ActivateExpansionShop.SetActive(true);
                expansionShopActivated = true;
            }
        }
        if (totalCashValue >= 250000 && !monopolyShopActivated)
        {
            if (ActivateMonopolyShop != null)
            {
                ActivateMonopolyShop.SetActive(true);
                monopolyShopActivated = true;
            }
        }
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
            //SaveMoney();
        }
        else
        {
            // Optionally handle the case when there isn't enough cash
        }
    }

    public void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = totalCashValue.ToString();
        }
    }
}
