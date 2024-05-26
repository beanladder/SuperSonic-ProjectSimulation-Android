using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WallAnimation : MonoBehaviour
{
    // References to your wall GameObjects
    public GameObject wall1;
    public GameObject wall2;
    public GameObject wall3;
    public GameObject ROOF1;
    public GameObject wall4;
    public GameObject wall5;
    public GameObject wall6;
    public GameObject ROOF2;
    public float animationDuration = 2.0f;
    public GameObject Shutter;
    void Start()
    {
        // Animate each wall's scale on the Y-axis to 0 over the specified duration
        
        AnimateShutter(Shutter);
    }

    void AnimateWall(GameObject wall)
    {
        if (wall != null)
        {
            wall.transform.DOScaleY(0f, animationDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => DisableGameObject(wall));
        }
    }

    void AnimateRoof(GameObject Roof)
    {
        if (Roof != null)
        {
            Roof.transform.DOScaleX(0f, animationDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => DisableGameObject(Roof));
        }
    }

    void AnimateShutter(GameObject shutter)
    {
        if (shutter != null)
        {
            // Ensure the initial scale is set to 0 on the Y-axis
            shutter.transform.localScale = new Vector3(shutter.transform.localScale.x, 1f, shutter.transform.localScale.z);

            // Animate the shutter's scale on the Y-axis from 0 to 1
            shutter.transform.DOScaleY(0f, animationDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => {
                    // After shutter animation is complete, perform other animations
                    AnimateWall(wall1);
                    AnimateWall(wall2);
                    AnimateWall(wall3);
                    AnimateRoof(ROOF1);
                    DisableGameObject(Shutter);
                });
        }
    }

    void DisableGameObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
