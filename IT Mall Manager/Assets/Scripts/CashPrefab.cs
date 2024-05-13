using UnityEngine;
using DG.Tweening;

public class CashPrefab : MonoBehaviour
{
    private Transform playerTransform;

    void Update()
    {
        if (playerTransform != null)
        {
            // Move towards the player's position using DOTween
            transform.DOMove(playerTransform.position, 1f);
        }
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
}
