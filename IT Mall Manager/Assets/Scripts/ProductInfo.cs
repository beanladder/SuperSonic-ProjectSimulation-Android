using System.Collections;
using System.Collections.Generic;
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
    private GameManager gameManager;
    public Product[] products;

    public static ProductInfo instance;

    private bool allProductsUsedUp = false;

    private void Awake()
    {
        // Ensure products are initialized
        //InitializeProducts();
        
        // Find the GameManager by traversing up the hierarchy

        
        
    }

    public void SetGameManager(GameManager gm)
    {
        gameManager = gm;
         if (gameManager != null)
        {
            // Filter products by available shelf types
            FilterProductsByShelfTypes(gameManager.GetAvailableShelfTypes());
        }
        else
        {
            Debug.LogError("GameManager not found in the parent hierarchy.");
        }
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
            StartCoroutine(DestroyCoroutine());
        }
    }

    private IEnumerator DestroyCoroutine()
    {
        if (gameManager != null)
        {
            Debug.Log("Updating products from ProductInfo");
            gameManager.UpdateAvailableShelves();
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
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

    // Method to filter products based on available shelf types
    public void FilterProductsByShelfTypes(HashSet<Shelf.ShelfType> availableShelfTypes)
    {
        List<Product> filteredProducts = new List<Product>();

        foreach (var product in products)
        {
            if (availableShelfTypes.Contains((Shelf.ShelfType)System.Enum.Parse(typeof(Shelf.ShelfType), product.productName)))
            {
                filteredProducts.Add(product);
            }
        }

        products = filteredProducts.ToArray();
        InitializeProducts();

        // Check if all products are used up and destroy the box if necessary
        CheckAllProductsUsedUp();
        if (allProductsUsedUp)
        {
            Destroy(gameObject);
        }
    }
}
