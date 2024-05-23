using UnityEngine;

public class CanvasBillboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Cache the main camera reference
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Make the canvas face the camera
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, 
                             mainCamera.transform.rotation * Vector3.up);
        }
    }
}
