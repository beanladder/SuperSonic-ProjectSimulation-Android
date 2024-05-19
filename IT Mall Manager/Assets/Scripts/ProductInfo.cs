using UnityEngine;

public class ProductInfo : MonoBehaviour
{
    public int MaxProductsOfEachType=1;
    public int CpuNum, MBNum, RamNum;

    public GameObject ramPrefab;
    public GameObject cpuPrefab;
    public GameObject motherboardPrefab;

    public static ProductInfo instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CpuNum = MBNum = RamNum = MaxProductsOfEachType;
    }

    public void IncreaseMaxCapacity()
    {
        MaxProductsOfEachType++;


        if (CpuNum < MaxProductsOfEachType)
        {
            CpuNum++;
        }
        if (MBNum < MaxProductsOfEachType)
        {
            MBNum++;
        }
        if (RamNum < MaxProductsOfEachType)
        {
            RamNum++;
        }
    }

    
}
