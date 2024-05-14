using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPostion : MonoBehaviour
{
    public static GetPostion instance;

    public Transform playerTransform;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("cShelf");
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
        // Update player's transform every frame
        if (playerTransform != null)
        {
            // If playerTransform is not null, update the player's transform
            playerTransform = playerTransform.root;
        }
    }
}
