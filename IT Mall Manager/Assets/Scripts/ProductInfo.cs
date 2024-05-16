using UnityEngine;

public class ProductInfo : MonoBehaviour
{
    public int MaxProductsOfEachType;
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
    private void Update()
    {
        if(CpuNum ==0 && RamNum ==0 && MBNum == 0)
        {
            Destroy(gameObject);
        }
    }
}
