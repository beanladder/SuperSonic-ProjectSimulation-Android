using UnityEngine;

[System.Serializable]
public class Product
{
    public string productName;
    public GameObject productPrefab;
    public int count = 1;
    public int defaultCount = 1; // Add default count
}

public class ProductInfo : MonoBehaviour
{
    public Product[] products;
    public static ProductInfo instance;
    public bool isAI = false;

    private bool allProductsUsedUp = false;

    private void Awake()
    {
        
        instance = this;
        InitializeProducts();
        
    }

    private void InitializeProducts()
    {
        // Initialize all product counts to their default values
        foreach (var product in products)
        {
            product.count = product.defaultCount;
        }
    }

    private void Update()
    {
        if (allProductsUsedUp)
        {
            Destroy(gameObject);
        }
    }

    public bool TryDecrementProductCount(string productName)
    {
        foreach (var product in products)
        {
            if (product.productName == productName)
            {
                if (product.count > 0)
                {
                    product.count--;
                    Debug.Log($"{productName} count decremented. New count: {product.count}");

                    CheckAllProductsUsedUp();
                    return true;
                }
                else
                {
                    Debug.LogWarning($"{productName} count is already zero. Cannot decrement.");
                    return false;
                }
            }
        }
        Debug.LogError($"{productName} not found in product list.");
        return false;
    }

    private void CheckAllProductsUsedUp()
    {
        allProductsUsedUp = true;
        foreach (var product in products)
        {
            if (product.count > 0)
            {
                allProductsUsedUp = false;
                break;
            }
        }
    }

    public Product GetProduct(string productName)
    {
        foreach (var product in products)
        {
            if (product.productName == productName)
            {
                return product;
            }
        }
        Debug.LogError($"{productName} not found in product list.");
        return null;
    }
}
