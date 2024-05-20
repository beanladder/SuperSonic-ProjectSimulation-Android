using UnityEngine;
using DG.Tweening;
using System.Transactions;

public class SuperContainer : MonoBehaviour
{
    public static SuperContainer Instance { get; private set; }

    public Transform playerHands;
    public Transform workerAIHands;
    public Transform[] spawnPoints;
    public bool PlayerEmptyHand = false;
    public bool WorkerEmptyHand = false;
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
            PlayerEmptyHand = true;
            CharacterMovementManager.instance.anim.SetLayerWeight(1, 0);
        }
        else
        {
            CharacterMovementManager.instance.anim.SetLayerWeight(1, 1);
        }

        if (workerAIHands.childCount < 1)
        {
            WorkerEmptyHand = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerEmptyHand)
            {
                MoveBoxToPlayer();
            }
            else
            {
                Debug.Log("Player already has a box in their hands.");
            }
        }
        else if (other.CompareTag("WorkerAI"))
        {
            if (WorkerEmptyHand)
            {
                MoveBoxToWorkerAi();
            }
            else
            {
                Debug.Log("Worker already has a box in their hands.");
            }
        }
    }

    private void MoveBoxToPlayer()
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
                    heldPackage = box.gameObject;
                    ProductInfo product = heldPackage.GetComponent<ProductInfo>();
                    product.isAI = false;
                    PlayerEmptyHand = false;
                });
                break;
            }
        }
    }

    public void MoveBoxToWorkerAi()
    {
        Debug.Log("Attempting to move box to WorkerAI.");
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount > 0)
            {
                Transform box = spawnPoint.GetChild(0);
                Debug.Log("Found a box at spawn point: " + spawnPoint.name);
                box.DOMove(workerAIHands.position, .1f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    box.SetParent(workerAIHands);
                    box.localPosition = Vector3.zero;
                    box.localRotation = Quaternion.identity;
                    heldPackage = box.gameObject;
                    if(heldPackage == null)
                    {
                        Debug.Log("problem in SuperContainer.MoveBoxToWorkerAi");
                    }
                    ProductInfo product = heldPackage.GetComponent<ProductInfo>();
                    product.isAI = true;
                    WorkerEmptyHand = false;
                    Debug.Log("Box moved to WorkerAI hands: " + heldPackage.name);
                });
                break;
            }
        }
    }
}
