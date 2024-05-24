using UnityEngine;

public class ProductInfo : MonoBehaviour
{
    public int playerMaxProducts=1; // Max products for player
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
        UpdateProductCounts();
    }

   
    // Upgrade methods for player and AI
    public void UpgradePlayerProductLimit(int newMaxProducts)
    {
        playerMaxProducts = newMaxProducts;
        if (!isAI)
        {
            UpdateProductCounts();
        }
    }

    public void UpgradeWorkerProductLimit(int newMaxProducts)
    {
        aiMaxProducts = newMaxProducts;
        if (isAI)
        {
            UpdateProductCounts();
        }
    }

    // Update product counts based on current max limits
    public void UpdateProductCounts()
    {
        int maxProducts = isAI ? aiMaxProducts : playerMaxProducts;
        CpuNum = MBNum = RamNum = maxProducts;
    }

    private void Update()
    {

        if (CpuNum < 1 && RamNum < 1 && MBNum < 1)
        {
            Destroy(gameObject);
        }
    }
}