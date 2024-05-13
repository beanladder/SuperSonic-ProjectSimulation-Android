using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour
{
    public VariableJoystick joystick;
    public CharacterController controller;
    public Canvas canvas;
    public bool isJoystick;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public Animator anim;
    private float timeElapsed = 0f;
    private bool speedIncreased = false;

    private void Start()
    {
        EnableJoystickInput();
    }

    public void EnableJoystickInput()
    {
        isJoystick = true;
        canvas.gameObject.SetActive(true);
    }

    private void Update()
    {
        

        if (isJoystick)
        {
            Vector3 movementDir = new Vector3(joystick.Direction.x, 0.0f, joystick.Direction.y);

            // Calculate target speed based on input magnitude
            float targetSpeed = movementDir.magnitude * moveSpeed;

            // Clamp target speed based on move speed limit
            targetSpeed = Mathf.Clamp(targetSpeed, 0f, moveSpeed);

            // Update character's position
            controller.SimpleMove(controller.transform.forward * targetSpeed);

            // Rotate towards movement direction
            Vector3 targetDir = Vector3.RotateTowards(controller.transform.forward, movementDir, rotationSpeed * Time.deltaTime, maxMagnitudeDelta: 0.0f);
            controller.transform.rotation = Quaternion.LookRotation(targetDir);

            // Set animation parameter based on current speed
            float mappedMove = targetSpeed / moveSpeed; // Map current speed to range [0, 1]
            anim.SetFloat("move", mappedMove);

        }
    }
}
