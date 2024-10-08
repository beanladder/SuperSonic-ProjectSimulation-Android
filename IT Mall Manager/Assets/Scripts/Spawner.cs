using UnityEngine;
using System.Collections;
public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform[] spawnPoints;
    public float popInDuration = 0.5f;
    public AnimationCurve popInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float checkInterval = 0.5f; // Interval to check for missing children
    public Quaternion prefabRotation;
    public GameManager gameManager; // Reference to the GameManager object

    private void Start()
    {
        SpawnAllPrefabs();

        // Start a coroutine to periodically check for missing children
        StartCoroutine(CheckForMissingChildren());
    }
    
    private void SpawnAllPrefabs()
    {
        // Check if the spawnPoints array is empty
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        // Iterate through each spawn point
        foreach (Transform spawnPoint in spawnPoints)
        {
            // Retrieve the local scale of the prefabToSpawn
            Vector3 prefabScale = prefabToSpawn.transform.localScale;
            prefabRotation = prefabToSpawn.transform.localRotation;
            // Spawn the prefab at the position of the spawn point with a small scale
            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPoint.position, prefabRotation);
            spawnedObject.transform.localScale = prefabScale;

            // Start the pop in animation and then change parent
            StartCoroutine(PopInAndChangeParent(spawnedObject.transform, spawnPoint, prefabScale, prefabRotation));

            // Pass the GameManager reference to the spawned prefab
            ProductInfo productInfo = spawnedObject.GetComponent<ProductInfo>();
            if (productInfo != null)
            {
                productInfo.SetGameManager(gameManager);
            }
        }
    }

    private IEnumerator PopInAndChangeParent(Transform targetTransform, Transform newParent, Vector3 targetScale, Quaternion targetRotation)
    {
        float timer = 0f;

        while (timer < popInDuration)
        {
            float scale = popInCurve.Evaluate(timer / popInDuration);
            targetTransform.localScale = targetScale * scale;
            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure the scale is exactly 1 at the end of animation
        targetTransform.localScale = targetScale;
        targetTransform.localRotation = targetRotation;
        // Change the parent after pop in animation
        targetTransform.SetParent(newParent);
    }

    private IEnumerator CheckForMissingChildren()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            // Iterate through each spawn point
            foreach (Transform spawnPoint in spawnPoints)
            {
                // Check if the spawn point has no children
                if (spawnPoint.childCount == 0)
                {
                    // If a spawn point has no children, spawn a new prefab
                    SpawnPrefabAtSpawnPoint(spawnPoint);
                }
            }
        }
    }

    private void SpawnPrefabAtSpawnPoint(Transform spawnPoint)
    {
        // Retrieve the local scale and rotation of the prefabToSpawn
        Vector3 prefabScale = prefabToSpawn.transform.localScale;
        Quaternion prefabRotation = prefabToSpawn.transform.localRotation;

        // Spawn the prefab at the position of the spawn point with a small scale and the prefab's rotation
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPoint.position, prefabRotation);
        spawnedObject.transform.localScale = prefabScale;

        // Start the pop in animation and then change parent
        StartCoroutine(PopInAndChangeParent(spawnedObject.transform, spawnPoint, prefabScale, prefabRotation));

        // Pass the GameManager reference to the spawned prefab
        ProductInfo productInfo = spawnedObject.GetComponent<ProductInfo>();
        if (productInfo != null)
        {
            productInfo.SetGameManager(gameManager);
        }
    }
}
