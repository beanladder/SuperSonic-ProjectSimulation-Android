using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPostion : MonoBehaviour
{
    public static GetPostion instance;

    public Transform playerTransform;
    public float yValue;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
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
        transform.position = new Vector3(playerTransform.position.x ,yValue,playerTransform.position.z);
    }
}
