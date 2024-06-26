using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading;

public class CashOutflow : MonoBehaviour
{
    public static CashOutflow instance;
    [SerializeField] Transform[] CashHolders; // Array to hold the 6 empty GameObject transforms
    [SerializeField] GameObject CashPrefab; // Prefab of the cash
    public float CashDeliveryTime, Yaxis; // Time between each cash spawn and y-axis offset
    public Vector3 DesiredRotation;

    public int cashToOutflow;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        

        //StartCoroutine(CashSpawn(CashDeliveryTime));

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
            Yaxis = CashHolders[cashIndex].childCount * 0.035f;
            // Set the scale and rotation of the new cash to match the original scale and rotation
            newCash.transform.localScale = originalScale;
            newCash.transform.rotation = Quaternion.Euler(DesiredRotation);

            // Set the parent of the new cash to the respective cash holder
            newCash.transform.parent = CashHolders[cashIndex];

            // Move the cash to the respective cash holder with a jump animation
            newCash.transform.DOJump(CashHolders[cashIndex].position + Vector3.up * Yaxis, .5f, 1, 0.5f).SetEase(Ease.OutQuad);
            // Increment cash index and adjust y-axis offset if needed
            cashIndex = (cashIndex + 1) % CashHolders.Length; // Wrap around to the beginning if index exceeds the array length
            


            countCash++;
            yield return new WaitForSeconds(time);
        }
    }
}