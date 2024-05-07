using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTrigger : MonoBehaviour
{
    public Animator buildingAnimator; // Reference to the Animator component of your building

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assuming your player has a tag "Player"
        {
            buildingAnimator.SetTrigger("Rise"); // Start the animation
        }
    }
}
