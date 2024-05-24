using System.Collections;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public enum ShelfType { CPU, RAM, Motherboard }
    public ShelfType shelfType;

    private Quaternion[] prefabRotations;
    private Vector3[] prefabScales;
    public int productCount; // Keeps track of the number of products on the shelf

    public int maxProducts; // Maximum number of products that can be on the shelf

    public float spawnDelay = 1f; // Delay between spawning prefabs

    public AnimationCurve popInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // Animation curve for pop-in animation
    public float popInDuration = 0.5f; // Duration of the pop-in animation

    private bool isAnimating = false; // Flag to track if pop-in animation is ongoing
    public bool isworker = false;

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
        //Debug.LogWarning(other.gameObject.name);
        
        if (other.CompareTag("Player") || other.CompareTag("WorkerAI"))
        {
            ProductInfo productInfo = other.GetComponentInChildren<ProductInfo>();
            isworker = true;
            if (productInfo != null)
            {
                switch (shelfType)
                {
                    case ShelfType.CPU:
                        if (productInfo.CpuNum > 0)
                        {
                            StartCoroutine(SpawnProductWithDelay(productInfo.cpuPrefab, productInfo, () => { }));
                        }
                        break;
                    case ShelfType.RAM:
                        if (productInfo.RamNum > 0)
                        {
                            StartCoroutine(SpawnProductWithDelay(productInfo.ramPrefab, productInfo, () => { }));
                        }
                        break;
                    case ShelfType.Motherboard:
                        if (productInfo.MBNum > 0)
                        {
                            StartCoroutine(SpawnProductWithDelay(productInfo.motherboardPrefab, productInfo, () => { }));
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

    private IEnumerator SpawnProductWithDelay(GameObject prefab, ProductInfo productInfo, System.Action onAllProductsSpawned)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(spawnDelay);

        bool spawnSuccessful = false;

        switch (shelfType)
        {
            case ShelfType.CPU:
                if (productInfo.CpuNum > 0)
                {
                    spawnSuccessful = SpawnProduct(prefab);
                    if (spawnSuccessful)
                        productInfo.CpuNum--;
                }
                break;
            case ShelfType.RAM:
                if (productInfo.RamNum > 0)
                {
                    spawnSuccessful = SpawnProduct(prefab);
                    if (spawnSuccessful)
                        productInfo.RamNum--;
                }
                break;
            case ShelfType.Motherboard:
                if (productInfo.MBNum > 0)
                {
                    spawnSuccessful = SpawnProduct(prefab);
                    if (spawnSuccessful)
                        productInfo.MBNum--;
                }
                break;
        }
    }

    // public void Restock(ProductInfo productInfo)
    // {
    //     // Assuming each restock adds one of each product type to the shelf.
    //     int restockedProducts = Mathf.Min(productInfo.CpuNum, productInfo.RamNum, productInfo.MBNum);
    //     productCount += restockedProducts;
    //     productCount = Mathf.Min(productCount, maxProducts); // Ensure productCount doesn't exceed maxProducts
    // }

    private IEnumerator PopInAnimation(Transform product)
    {
        isAnimating = true;
        float elapsedTime = 0f;

        Vector3 initialScale = new Vector3(0f, 0f, 0f);
        Vector3 targetScale = product.localScale;

        while (elapsedTime < popInDuration)
        {
            float scale = popInCurve.Evaluate(elapsedTime / popInDuration);
            product.localScale = targetScale * scale;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        product.localScale = targetScale;
        isAnimating = false;
    }

    private bool SpawnProduct(GameObject prefab)
    {
        if (isAnimating || productCount >= maxProducts)
            return false; // Return false if already animating or shelf is full

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

                StartCoroutine(PopInAnimation(product.transform)); // Start pop-in animation

                return true; // Exit the method after starting pop-in animation
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

    public bool RemoveProduct()
    {
        Transform[] spawnPoints = GetSpawnPoints();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount > 0) // Check if spawn point has a child
            {
                Destroy(spawnPoint.GetChild(0).gameObject, 2f); // Remove the product from the shelf
                productCount--;
                return true;
            }
        }
        return false;
    }
}

