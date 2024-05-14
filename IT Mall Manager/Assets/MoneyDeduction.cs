using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoneyDeduction : MonoBehaviour
{
    public int deductionAmount = 100; // Amount of money to deduct when player is in range
    public float deductionInterval = 1f;
    public bool playerInRange = false; // Flag to track if player is in range
    private Coroutine deductionCoroutine;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            //DeductMoney();
            deductionCoroutine = StartCoroutine(DeductMoneyOverTime());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if(deductionCoroutine!=null){
                StopCoroutine(deductionCoroutine);
            }
        }
    }
    
    IEnumerator DeductMoneyOverTime(){

        // Check if player has enough cash to deduct
            if (PlayerCashCounter.instance.totalCashValue >= deductionAmount)
            {
                // Deduct the specified amount
                PlayerCashCounter.instance.DeductTotalCash(deductionAmount);
                Debug.Log("Deducted " + deductionAmount + " from player's cash.");
            }
            else
            {
                Debug.Log("Not enough cash to deduct " + deductionAmount + ".");
            }

            // Wait for the next deduction interval
            yield return new WaitForSeconds(deductionInterval);
        }

    // void DeductMoney()
    // {
    //     if (PlayerCashCounter.instance.totalCashValue >= deductionAmount)
    //     {
    //         PlayerCashCounter.instance.DeductTotalCash(deductionAmount);
    //         Debug.Log("Deducted " + deductionAmount + " from player's cash.");
    //     }
    //     else
    //     {
    //         Debug.Log("Not enough cash to deduct " + deductionAmount + ".");
    //     }
    // }
}
