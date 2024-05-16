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
                switch (shelfType)
                {
                    case ShelfType.CPU:
                        if (productInfo.CpuNum > 0)
                        {
                            productInfo.CpuNum--;
                            SpawnProduct(productInfo.cpuPrefab);
                        }
                        break;
                    case ShelfType.RAM:
                        if (productInfo.RamNum > 0)
                        {
                            productInfo.RamNum--;
                            SpawnProduct(productInfo.ramPrefab);
                        }
                        break;
                    case ShelfType.Motherboard:
                        if (productInfo.MBNum > 0)
                        {
                            productInfo.MBNum--;
                            SpawnProduct(productInfo.motherboardPrefab);
                        }
                        break;
                    default:
                        Debug.LogWarning("Unknown shelf type.");
                        break;
                }
            }
            else
            {
                Debug.LogWarning("ProductInfo not found on player.");
            }
        }
    }

    private void SpawnProduct(GameObject prefab)
    {
        Transform[] spawnPoints = GetComponentsInChildren<Transform>();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != transform && spawnPoint.childCount == 0)
            {
                GameObject product = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
                product.transform.localScale = prefabScales[(int)shelfType];
                product.transform.localRotation = prefabRotations[(int)shelfType];
                product.transform.parent = spawnPoint;
                Debug.Log("Product spawned on shelf.");
                break;
            }
        }
    }

}
