using UnityEngine;
using DG.Tweening;

public class SuperContainer : MonoBehaviour
{
    public static SuperContainer Instance { get; private set; }

    public Transform playerHands;
    public Transform[] spawnPoints;
    public bool EmptyHand = false;
    public GameObject heldPackage; // Stores the currently held package

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of SuperContainer found. Make sure there is only one.");
            Destroy(gameObject);
        }

        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }
    }

    private void Update()
    {
        if (playerHands.childCount < 1)
        {
            EmptyHand = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (EmptyHand)
            {
                MoveBoxToPlayer(playerHands.transform);
            }
            else
            {
                Debug.Log("Player already has a box in their hands.");
            }
        }
    }

    private void MoveBoxToPlayer(Transform player)
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount > 0)
            {
                Transform box = spawnPoint.GetChild(0);
                box.DOMove(playerHands.position, .1f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    box.SetParent(playerHands);
                    box.localPosition = Vector3.zero;
                    box.localRotation = Quaternion.identity;
                    heldPackage = box.gameObject; // Set the currently held package
                    EmptyHand = false;
                });
                break;
            }
        }
    }
}
