using UnityEngine;

public class DestroyPrefab : MonoBehaviour
{
    private CashMovement cashMovement;

    private void Start()
    {
        cashMovement = FindObjectOfType<CashMovement>(); // Find the CashMovement script in the scene
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            //Destroy(gameObject); // Destroy the cash prefab
        }
    }
}
