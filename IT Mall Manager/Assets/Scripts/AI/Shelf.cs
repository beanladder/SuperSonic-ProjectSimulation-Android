using System.Collections;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public enum ShelfType { CPU, RAM, Motherboard, Phone, Laptop, Macbook, RCPU, RRAM, RMotherboard, GPU, PC }
    public ShelfType shelfType;

    private Quaternion[] prefabRotations;
    private Vector3[] prefabScales;
    public int productCount; // Keeps track of the number of products on the shelf

    public int maxProducts; // Maximum number of products that can be on the shelf

    public float spawnDelay = 1f; // Delay between spawning prefabs

    public AnimationCurve popInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // Animation curve for pop-in animation
    public float popInDuration = 0.5f; // Duration of the pop-in animation

    private bool isAnimating = false; // Flag to track if pop-in animation is ongoing

    private void Awake()
    {
        prefabRotations = new Quaternion[ProductInfo.instance.products.Length];
        prefabScales = new Vector3[ProductInfo.instance.products.Length];

        for (int i = 0; i < ProductInfo.instance.products.Length; i++)
        {
            prefabRotations[i] = ProductInfo.instance.products[i].productPrefab.transform.localRotation;
            prefabScales[i] = ProductInfo.instance.products[i].productPrefab.transform.localScale;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("WorkerAI"))
        {
            ProductInfo productInfo = other.GetComponentInChildren<ProductInfo>();
            if (productInfo != null)
            {
                foreach (var product in productInfo.products)
                {
                    if (product.productName == shelfType.ToString() && product.count > 0)
                    {
                        StartCoroutine(SpawnProductWithDelay(product.productPrefab, productInfo, () => { }));
                    }
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
        yield return new WaitForSeconds(spawnDelay);

        bool spawnSuccessful = false;

        foreach (var product in productInfo.products)
        {
            if (product.productName == shelfType.ToString() && product.count > 0)
            {
                spawnSuccessful = SpawnProduct(prefab);
                if (spawnSuccessful)
                {
                    productInfo.TryDecrementProductCount(product.productName);
                }
                break;
            }
        }
    }

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
