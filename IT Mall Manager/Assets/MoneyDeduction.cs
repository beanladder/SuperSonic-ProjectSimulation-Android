using UnityEngine;

public class MoneyDeduction : MonoBehaviour
{
    public int deductionAmount = 100; // Amount of money to deduct when player is in range
    public bool playerInRange = false; // Flag to track if player is in range

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            DeductMoney();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void DeductMoney()
    {
        if (PlayerCashCounter.instance.totalCashValue >= deductionAmount)
        {
            PlayerCashCounter.instance.DeductTotalCash(deductionAmount);
            Debug.Log("Deducted " + deductionAmount + " from player's cash.");
        }
        else
        {
            Debug.Log("Not enough cash to deduct " + deductionAmount + ".");
        }
    }
}
