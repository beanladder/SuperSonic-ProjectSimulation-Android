using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public Transform carryTransform; // Reference to the player's hand Transform

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (carryTransform.childCount > 0)
            {
                Transform childToDestroy = carryTransform.GetChild(0); // Get the first child of carryTransform
                Destroy(childToDestroy.gameObject); // Destroy the child GameObject
            }
        }
    }
}
