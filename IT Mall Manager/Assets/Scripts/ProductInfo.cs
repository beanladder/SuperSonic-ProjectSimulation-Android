using UnityEngine;

public class ProductInfo : MonoBehaviour
{
    public int playerMaxProducts; // Max products for player
    public int aiMaxProducts; // Max products for AI
    public int CpuNum, MBNum, RamNum;
    public GameObject ramPrefab;
    public GameObject cpuPrefab;
    public GameObject motherboardPrefab;
    public static ProductInfo instance;
    public bool isAI = false;
    public GameObject workerGO;
    public WorkerAI workerAI;
    private void Awake()
    {   
        workerAI = workerGO.GetComponent<WorkerAI>();
        instance = this;
        UpdateProductCounts();
    }

    public void UpgradeButton()
    {
        workerAI.Upgrade();
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