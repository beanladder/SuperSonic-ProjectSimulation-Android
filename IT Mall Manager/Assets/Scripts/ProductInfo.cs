using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductInfo : MonoBehaviour
{
    public enum ProductType { CPU, RAM, Motherboard }

    public static ProductInfo Instance { get; private set; } // Singleton instance

    public int cpuCount = 1;
    public int ramCount = 1;
    public int motherboardCount = 1;

    // ... rest of the code from the previous response (ReduceProductCount functions)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple ProductInfo instances detected! Consider using a different approach.");
        }
    }
    public bool ReduceProductCount(ProductType productType)
    {
        switch (productType)
        {
            case ProductType.CPU:
                if (cpuCount > 0)
                {
                    cpuCount--;
                    return true;
                }
                break;
            case ProductType.RAM:
                if (ramCount > 0)
                {
                    ramCount--;
                    return true;
                }
                break;
            case ProductType.Motherboard:
                if (motherboardCount > 0)
                {
                    motherboardCount--;
                    return true;
                }
                break;
            default:
                Debug.LogError("Invalid product type provided to ReduceProductCount");
                break;
        }

        return false; // No product count was reduced
    }

    // Alternative function (optional): Reduce product count by one for any type
    public bool ReduceAnyProductCount()
    {
        // Prioritize reducing CPU, then RAM, then Motherboard (adjust order as needed)
        if (ReduceProductCount(ProductType.CPU))
        {
            return true;
        }

        if (ReduceProductCount(ProductType.RAM))
        {
            return true;
        }

        if (ReduceProductCount(ProductType.Motherboard))
        {
            return true;
        }

        return false; // No product of any type was available to reduce
    }
}
