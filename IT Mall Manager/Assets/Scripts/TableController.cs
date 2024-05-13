using UnityEngine;

public class TableController : MonoBehaviour
{
    private Transform playerTransform;
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = null;
            playerInRange = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerInRange)
        {
            CashPrefab[] cashPrefabs = GetComponentsInChildren<CashPrefab>(true);

            foreach (CashPrefab cashPrefab in cashPrefabs)
            {
                cashPrefab.SetPlayerTransform(playerTransform);
            }
        }
    }
}
