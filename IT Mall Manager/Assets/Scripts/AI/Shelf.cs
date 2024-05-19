using System.Collections;
using UnityEngine;

public class Shelf : MonoBehaviour
{
   public enum ShelfType { CPU, RAM, Motherboard }
    public ShelfType shelfType;

    private Quaternion[] prefabRotations;
    private Vector3[] prefabScales;
    public int productCount; // Keeps track of the number of products on the shelf

    private void Awake()
    {
        // Initialize the rotations and scales for different products
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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            ProductInfo productInfo = player.GetComponentInChildren<ProductInfo>();

            if (productInfo != null)
            {
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

    public bool RemoveProduct()
    {
        Transform[] spawnPoints = GetSpawnPoints();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount > 0) // Check if spawn point has a child
            {
                Destroy(spawnPoint.GetChild(0).gameObject); // Remove the product from the shelf
                productCount--;
                return true;
            }
        }
        return false;
    }

    private bool SpawnProduct(GameObject prefab)
    {
        Transform[] spawnPoints = GetSpawnPoints();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount == 0) // Check if spawn point has no child
            {
                GameObject product = Instantiate(prefab, spawnPoint.position, prefab.transform.localRotation);
                product.transform.localScale = prefab.transform.localScale;
                product.transform.localRotation = prefab.transform.localRotation;
                product.transform.parent = spawnPoint;
                productCount++;
                return true; // Exit the method after spawning a product
            }
        }
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

    public GameObject GetProductPrefab()
    {
        switch (shelfType)
        {
            case ShelfType.CPU:
                return ProductInfo.instance.cpuPrefab;
            case ShelfType.RAM:
                return ProductInfo.instance.ramPrefab;
            case ShelfType.Motherboard:
                return ProductInfo.instance.motherboardPrefab;
            default:
                return null;
        }
    }
}
