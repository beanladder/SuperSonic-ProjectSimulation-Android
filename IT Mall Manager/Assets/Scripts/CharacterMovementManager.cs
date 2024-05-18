using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour
{
    public static CharacterMovementManager instance;
    public VariableJoystick joystick;
    public CharacterController controller;
    public Canvas canvas;
    public bool isJoystick;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float decelerationRate = 100f; // New deceleration rate
    public Animator anim;
    private float currentSpeed = 0f; // Current speed of the character


    private void Awake()
    {
        instance = this;
    }
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

            if (targetSpeed > 0f)
            {
                // Update current speed towards target speed
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, moveSpeed);
            }
            else
            {
                // Decelerate the character when there's no input
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, decelerationRate * Time.deltaTime);
            }

            // Update character's position
            controller.SimpleMove(controller.transform.forward * currentSpeed);

            if (movementDir != Vector3.zero)
            {
                // Rotate towards movement direction
                Vector3 targetDir = Vector3.RotateTowards(controller.transform.forward, movementDir, rotationSpeed * Time.deltaTime, maxMagnitudeDelta: 0.0f);
                controller.transform.rotation = Quaternion.LookRotation(targetDir);
            }

            // Set animation parameter based on current speed
            float mappedMove = currentSpeed / moveSpeed; // Map current speed to range [0, 1]
            anim.SetFloat("move", mappedMove);
        }
    }
}
