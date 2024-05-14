using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CashOutflow : MonoBehaviour
{
    [SerializeField] Transform[] CashHolders = new Transform[6]; // Array to hold the 6 empty GameObject transforms
    [SerializeField] GameObject CashPrefab; // Prefab of the cash
    [SerializeField] float CashDeliveryTime, Yaxis; // Time between each cash spawn and y-axis offset
    
    public int cashToOutflow = 200;

    void Start()
    {
        StartCoroutine(CashSpawn(CashDeliveryTime));
    }

    public void SetCashToOutflow(int amount)
    {
        cashToOutflow = amount;
    }
    public IEnumerator CashSpawn(float time)
    {
        int cashIndex = 0; // Index to track the current cash holder
        int countCash = 0; // Counter to track the total number of cash spawned
        Vector3 originalScale = CashPrefab.transform.localScale; // Store the original scale
        Quaternion originalRotation = CashPrefab.transform.rotation; // Store the original rotation

        while (countCash < cashToOutflow) // You can adjust the condition as needed
        {
            GameObject newCash = Instantiate(CashPrefab, transform.position, Quaternion.identity); // Instantiate cash at the dispenser position

            // Set the scale and rotation of the new cash to match the original scale and rotation
            newCash.transform.localScale = originalScale;
            newCash.transform.rotation = originalRotation;

            // Set the parent of the new cash to the respective cash holder
            newCash.transform.parent = CashHolders[cashIndex];

            // Move the cash to the respective cash holder with a jump animation
            newCash.transform.DOJump(CashHolders[cashIndex].position + Vector3.up * Yaxis, .5f, 1, 0.5f).SetEase(Ease.OutQuad);

            // Increment cash index and adjust y-axis offset if needed
            cashIndex = (cashIndex + 1) % CashHolders.Length; // Wrap around to the beginning if index exceeds the array length
            if (cashIndex == 0) // Increase Y-axis offset if a full cycle of cash holders is completed
            {
                Yaxis += 0.035f; // You can adjust this value as needed
            }

            countCash++; // Increment total cash count
            yield return new WaitForSeconds(time); // Wait for the specified time before spawning the next cash
        }
    }


}
