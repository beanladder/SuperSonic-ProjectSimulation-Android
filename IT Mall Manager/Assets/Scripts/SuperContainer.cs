using UnityEngine;
using DG.Tweening;

public class SuperContainer : MonoBehaviour
{
    // Reference to the player's hands or the object where you want to move the boxes
    public Transform playerHands;

    // Reference to the spawn points
    public Transform[] spawnPoints;

    void Start()
    {
        // Get all the spawn points as children of the shelf
        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the player
        if (other.CompareTag("Player"))
        {
            // Check if player's hands are empty
            if (playerHands.childCount == 0)
            {
                // Move one box to player's hands using DOTween
                MoveBoxToPlayer();
            }
            else
            {
                Debug.Log("Player already has a box in their hands.");
            }
        }
    }

    private void MoveBoxToPlayer()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            // Iterate through each spawn point and move the box to the player's hands using DOTween
            if (spawnPoint.childCount > 0)
            {
                Transform box = spawnPoint.GetChild(0); // Assuming only one box per spawn point
                // Tween the box's position to the player's hand position
                box.DOMove(playerHands.position, .1f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    // Once tweening is complete, parent the box to the player's hands
                    box.SetParent(playerHands);
                    box.localPosition = Vector3.zero; // Optionally, reset the local position of the box
                });
                // Break out of the loop after moving one box
                break;
            }
        }
    }
}
