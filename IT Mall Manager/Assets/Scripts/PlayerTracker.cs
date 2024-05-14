using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public bool isPlayerInRange = false; // Boolean to check if player is in range
    private Transform playerTransform;
    public Animator anim;
    void Start()
    {
        // Assuming the player object has a tag "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        anim = playerObject.GetComponent<Animator>();
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found. Make sure to tag your player object with 'Player'.");
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Track player's position
            Vector3 playerPosition = playerTransform.position;

            // Do something with playerPosition, like print it
            //Debug.Log("Player Position: " + playerPosition);

            // Check if player is in range of the collider
            if (isPlayerInRange)
            {
                // Do something when player is in range
               // Debug.Log("Player is in range of the table.");
            }
        }
    }

    // OnTriggerEnter is called when the Collider other enters the trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            anim.SetLayerWeight(1, 1f);
        }
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            anim.SetLayerWeight(1, 0f);
        }
    }
}
