using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateController : MonoBehaviour
{
    void Start()
    {
        // Set the target frame rate to 120fps
        Application.targetFrameRate = 120;
    }
}
