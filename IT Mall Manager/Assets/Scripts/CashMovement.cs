using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
public class CashMovement : MonoBehaviour
{
    public static CashMovement instance;
    public Transform playerTransform;
    public Transform tableTransform;
    public float moveDuration = 12f;
    public float delayBetweenMovement = 0.0000000001f;
    public float cashReachedPlayer=0;
    public Text moneyText;
    private bool isInRange = false;
    private bool isMovingCash = false; // Flag to track whether cash movement coroutine is running
    private int cashValuePerPrefab = 500; // Value of each cash prefab
    void Awake(){
        instance=this;
    }
    void Update()
    {
        if (isInRange && !isMovingCash)
        {
            StartCoroutine(MoveCashTowardsPlayer());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    IEnumerator MoveCashTowardsPlayer()
    {
        isMovingCash = true;
        
        try
        {
            
            Transform[] spawnPoints = tableTransform.GetComponentsInChildren<Transform>();

            foreach (Transform spawnPoint in spawnPoints)
            {
                // Exclude the table's own transform
                if (spawnPoint == tableTransform)
                    continue;

                
                List<GameObject> cashObjects = new List<GameObject>();
                foreach (Transform child in spawnPoint)
                {
                    cashObjects.Add(child.gameObject);
                }

                
                cashObjects.Reverse();

                
                foreach (GameObject cashObject in cashObjects)
                {
                    if (cashObject != null)
                    {
                        cashObject.transform.DOJump(playerTransform.position, 2f, 1, moveDuration).SetEase(Ease.Linear).SetEase(Ease.OutQuad)
                            .OnComplete(() => { PlayerCashCounter.instance.IncreaseTotalCashReached(cashValuePerPrefab); }); 
                        yield return new WaitForSeconds(delayBetweenMovement); 
                        Destroy(cashObject);
                    }
                }
            }
        }
        finally
        {
            isMovingCash = false; // Reset the flag when coroutine completes or is interrupted
        }
    }

   
}
