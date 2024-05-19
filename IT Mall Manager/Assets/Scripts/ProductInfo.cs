using UnityEngine;

public class ProductInfo : MonoBehaviour
{
    public int playerMaxProducts = 2; // Default max products for player
    public int aiMaxProducts = 1; // Default max products for AI
    public int CpuNum, MBNum, RamNum;
    public GameObject ramPrefab;
    public GameObject cpuPrefab;
    public GameObject motherboardPrefab;
    public static ProductInfo instance;
    public bool isAI=false;

    private void Awake()
    {
        instance = this;
        UpdateProductCounts();
    }

    // Upgrade methods for player and AI
    public void UpgradePlayerProductLimit(int increaseAmount)
    {
        playerMaxProducts = increaseAmount;
        UpdateProductCounts();
    }

    public void UpgradeWorkerProductLimit(int increaseAmount)
    {
        aiMaxProducts =increaseAmount;
        UpdateProductCounts();
    }

    // Update individual product counts based on the appropriate max value
    private void UpdateProductCounts()
    {
        int maxProducts = isAI ? aiMaxProducts : playerMaxProducts; // Use ternary operator for conditional assignment
        CpuNum = MBNum = RamNum = Mathf.Min(maxProducts, MaxProductsPerType()); // Ensure limit doesn't exceed MaxProductsPerType()
    }

    // Function to define the maximum products per type (replace with your logic)
    private int MaxProductsPerType()
    {
        // Replace this with your logic to determine the maximum products per type
        // This example returns a fixed value of 10
        return 10;
    }
}
