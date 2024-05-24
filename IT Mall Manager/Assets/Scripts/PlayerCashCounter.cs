using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEditor.ShaderKeywordFilter;
public class PlayerCashCounter : MonoBehaviour
{
    public int totalCashValue = 0; // Total cash value reached by the player
    public TextMeshProUGUI moneyText;
    public static PlayerCashCounter instance;
    private const string MoneyKey = "PlayerMoney";

    private void Awake()
    {
        instance = this;
        //LoadMoney();
    }
    public void IncreaseTotalCashReached(int amount)
    {
        totalCashValue += amount; // Increase total cash value based on the amount and value per prefab
        UpdateMoneyUI();
        SaveMoney();
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
            
        }
    }

    public void UpdateMoneyUI(){
        if(moneyText!=null){
            moneyText.text = totalCashValue.ToString();
        }
    }
    private void SaveMoney(){
        PlayerPrefs.SetInt(MoneyKey,totalCashValue);
        PlayerPrefs.Save();
    }
    private void LoadMoney(){
        totalCashValue = PlayerPrefs.GetInt(MoneyKey,0);
        UpdateMoneyUI();
    }
}
