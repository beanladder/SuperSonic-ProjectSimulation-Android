using UnityEngine;

public class ProductInfo : MonoBehaviour
{
    public int playerMaxProducts=2; // Max products for player
    public int aiMaxProducts=1; // Max products for AI
    public int CpuNum, MBNum, RamNum;
    public GameObject ramPrefab;
    public GameObject cpuPrefab;
    public GameObject motherboardPrefab;
    public static ProductInfo instance;
    public bool isAI = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        int maxProducts = isAI ? aiMaxProducts : playerMaxProducts;
        CpuNum = RamNum = MBNum = maxProducts;
    }
    // Upgrade methods for player and AI
    public void UpgradePlayerProductLimit(int increaseAmount)
    {
        playerMaxProducts = increaseAmount;
        UpdateProductCounts();
    }

    public void UpgradeWorkerProductLimit(int increaseAmount)
    {
        aiMaxProducts = increaseAmount;
        UpdateProductCounts();
    }

    // Update individual product counts based on the appropriate max value
    private void UpdateProductCounts()
    {
        int maxProducts = isAI ? aiMaxProducts : playerMaxProducts; // Use ternary operator for conditional assignment
        CpuNum = MBNum = RamNum = maxProducts; // Ensure limit doesn't exceed MaxProductsPerType()
    }

    private void Update()
    {
        if(CpuNum<1 && RamNum<1 && MBNum < 1)
        {
            Destroy(gameObject);
        }
    }
}
