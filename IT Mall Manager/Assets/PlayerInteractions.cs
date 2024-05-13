using UnityEngine;
using DG.Tweening;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject ramPrefab;
    public GameObject cpuPrefab;
    public GameObject mbPrefab;
    public Transform playerHand;
    public static PlayerInteraction instance;
    private bool hasChildObject = false;
    private bool ramSpawned = false;
    private bool cpuSpawned = false;
    private bool mbSpawned = false;

    private void Awake()
    {
        instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasChildObject)
            return;

        if (other.CompareTag("rShelf") && !ramSpawned)
        {
            SpawnAndMovePrefab(ramPrefab, other);
            ramSpawned = true;
        }
        else if (other.CompareTag("cShelf") && !cpuSpawned)
        {
            SpawnAndMovePrefab(cpuPrefab, other);
            cpuSpawned = true;
        }
        else if (other.CompareTag("mShelf") && !mbSpawned)
        {
            SpawnAndMovePrefab(mbPrefab, other);
            mbSpawned = true;
        }

        if (ramSpawned && cpuSpawned && mbSpawned)
        {
            hasChildObject = false;
            DestroyChildObject();
        }
    }

    private void SpawnAndMovePrefab(GameObject prefab, Collider shelfCollider)
    {
        if (prefab == null)
            return;

        Transform spawnPoint = shelfCollider.transform.Find("SpawnPoint");
        if (spawnPoint != null)
        {
            GameObject spawnedPrefab = Instantiate(prefab, transform.position, Quaternion.identity);
            spawnedPrefab.transform.DOMove(spawnPoint.position, 1f).SetEase(Ease.OutQuad);
        }
    }

    private void DestroyChildObject()
    {
        if (playerHand.childCount > 0)
        {
            Destroy(playerHand.GetChild(0).gameObject);
        }
    }

    public void SetHasChildObject(bool value)
    {
        hasChildObject = value;
    }
}
