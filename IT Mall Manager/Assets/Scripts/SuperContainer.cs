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
            WorkerAI.instance.animator.SetLayerWeight(1, 0);
        }
        else
        {
            WorkerAI.instance.animator.SetLayerWeight(1, 1);
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
                MoveBoxToWorkerAI();
                
            }
            else
            {
                Debug.Log("Worker already has a box in their hands.");
            }
        }
    }

    public void MoveBoxToPlayer()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount > 0)
            {
                Transform box = spawnPoint.GetChild(0);
                box.DOMove(playerHands.position, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    box.SetParent(playerHands);
                    box.localPosition = Vector3.zero;
                    box.localRotation = Quaternion.identity;
                    heldPackage = box.gameObject;
                    ProductInfo product = heldPackage.GetComponent<ProductInfo>();
                    product.isAI = false;
                    product.UpdateProductCounts(); // Update product counts for player
                    PlayerEmptyHand = false;
                });
                break;
            }
        }
    }

    public void MoveBoxToWorkerAI()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount > 0)
            {
                Transform box = spawnPoint.GetChild(0);
                box.DOMove(workerAIHands.position, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    box.SetParent(workerAIHands);
                    box.localPosition = Vector3.zero;
                    box.localRotation = Quaternion.identity;
                    heldPackage = box.gameObject;
                    ProductInfo product = heldPackage.GetComponent<ProductInfo>();
                    product.isAI = true;
                    product.UpdateProductCounts(); // Update product counts for AI
                    WorkerEmptyHand = false;
                });
                break;
            }
        }
    }
}
