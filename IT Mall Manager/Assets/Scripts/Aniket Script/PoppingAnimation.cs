
using TMPro;
using UnityEngine;

public class PoppingAnimation : MonoBehaviour
{
    // The duration of the pop in and pop out animations
    public float popDuration = 0.5f;

    // The scale factor for the pop in animation
    public float popScale = 1.2f;
    public GameObject[] emoteGameObjects;
    private Vector3 originalScale;
    public float finalScale = 1.5f;
    private int currentEmoteIndex = -1;
    private bool isPressed = false; // Make isPressed a class variable

    void Awake()
    {
        // Cache the original scale
        originalScale = transform.localScale;

        // Ensure all emote GameObjects start hidden but active
        emoteGameObjects = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            emoteGameObjects[i] = transform.GetChild(i).gameObject;
            emoteGameObjects[i].transform.localScale = Vector3.zero;
            emoteGameObjects[i].SetActive(true); // Ensure they are active for scaling
        }
        for(int i =0;i<transform.childCount;i++){
            emoteGameObjects[i].SetActive(false);
        }
    }

    public void HideEmote(GameObject emote)
    {
        LeanTween.scale(emote, Vector3.zero, popDuration)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => {
                emote.SetActive(false);
                emote.transform.localScale = originalScale; // Reset scale for the next show
            });
    }

    public void ShowEmote(GameObject emote)
    {
        emote.SetActive(true);
        emote.transform.localScale = Vector3.zero;
        LeanTween.scale(emote, originalScale * popScale, popDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() => {
                LeanTween.scale(emote, originalScale*finalScale, popDuration * 0.5f)
                    .setEase(LeanTweenType.easeOutBack);
            });
    }

    public void Button()
    {
        if (!isPressed)
        {
            ShowEmote(emoteGameObjects[2]);
            isPressed = true;
        }
        else
        {
            HideEmote(emoteGameObjects[2]);
            isPressed = false;
        }
    }
}









































