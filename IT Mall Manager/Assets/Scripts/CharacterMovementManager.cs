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
        if(isJoystick)
        {
            Vector3 movementDir = new Vector3(joystick.Direction.x,0.0f,joystick.Direction.y);
            controller.SimpleMove(movementDir * moveSpeed);



            Vector3 targetDir = Vector3.RotateTowards(controller.transform.forward, movementDir, rotationSpeed * Time.deltaTime,maxMagnitudeDelta:0.0f);

            controller.transform.rotation = Quaternion.LookRotation(targetDir);
            if (movementDir.magnitude <= 0.1f)
            {
                anim.SetBool("IsMoving", false);
            }
            else
            {
                anim.SetBool("IsMoving", true);
            }
        }
    }
}
