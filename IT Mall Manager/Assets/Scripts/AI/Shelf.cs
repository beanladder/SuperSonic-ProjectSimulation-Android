using UnityEngine;

public class Shelf : MonoBehaviour
{
    public enum ShelfType { CPU, RAM, Motherboard }

    public ShelfType shelfType;

    private Quaternion[] prefabRotations;
    private Vector3[] prefabScales;

    private void Awake()
    {
        // Store the local scale and rotation of each prefab
        prefabRotations = new Quaternion[3];
        prefabScales = new Vector3[3];

        if (shelfType == ShelfType.CPU)
        {
            prefabRotations[0] = ProductInfo.instance.cpuPrefab.transform.localRotation;
            prefabScales[0] = ProductInfo.instance.cpuPrefab.transform.localScale;
        }
        else if (shelfType == ShelfType.RAM)
        {
            prefabRotations[1] = ProductInfo.instance.ramPrefab.transform.localRotation;
            prefabScales[1] = ProductInfo.instance.ramPrefab.transform.localScale;
        }
        else if (shelfType == ShelfType.Motherboard)
        {
            prefabRotations[2] = ProductInfo.instance.motherboardPrefab.transform.localRotation;
            prefabScales[2] = ProductInfo.instance.motherboardPrefab.transform.localScale;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            ProductInfo productInfo = player.GetComponentInChildren<ProductInfo>();

            if (productInfo != null)
            {
                Debug.Log("ProductInfo found on player.");
                bool spawnSuccessful = false;
                switch (shelfType)
                {
                    case ShelfType.CPU:
                        if (productInfo.CpuNum > 0)
                        {
                            spawnSuccessful = SpawnProduct(productInfo.cpuPrefab);
                        }
                        break;
                    case ShelfType.RAM:
                        if (productInfo.RamNum > 0)
                        {
                            spawnSuccessful = SpawnProduct(productInfo.ramPrefab);
                        }
                        break;
                    case ShelfType.Motherboard:
                        if (productInfo.MBNum > 0)
                        {
                            spawnSuccessful = SpawnProduct(productInfo.motherboardPrefab);
                        }
                        break;
                    default:
                        Debug.LogWarning("Unknown shelf type.");
                        break;
                }

                if (spawnSuccessful)
                {
                    // Decrease the product count only if spawn was successful
                    switch (shelfType)
                    {
                        case ShelfType.CPU:
                            productInfo.CpuNum--;
                            break;
                        case ShelfType.RAM:
                            productInfo.RamNum--;
                            break;
                        case ShelfType.Motherboard:
                            productInfo.MBNum--;
                            break;
                    }
                }
            }
            else
            {
                Debug.LogWarning("ProductInfo not found on player.");
            }
        }
    }

    private bool SpawnProduct(GameObject prefab)
    {
        // Find all children named "SpawnPoint" of this shelf
        Transform[] spawnPoints = GetSpawnPoints();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount == 0) // Check if spawn point has no child
            {
                GameObject product = Instantiate(prefab, spawnPoint.position, prefab.transform.localRotation);
                product.transform.localScale = prefab.transform.localScale;
                product.transform.localRotation = prefab.transform.localRotation;
                product.transform.parent = spawnPoint;
                Debug.Log("Product spawned on shelf.");
                return true; // Exit the method after spawning a product
            }
        }
        Debug.Log("No available spawn point for product.");
        return false; // Return false if no spawn occurred
    }





    private Transform[] GetSpawnPoints()
    {
        // Find all children named "SpawnPoint" of this shelf
        Transform[] spawnPoints = new Transform[transform.childCount];
        int index = 0;
        foreach (Transform child in transform)
        {
            if (child.name == "SpawnPoint")
            {
                spawnPoints[index] = child;
                index++;
            }
        }
        System.Array.Resize(ref spawnPoints, index); // Resize the array to remove null entries
        return spawnPoints;
    }
}
