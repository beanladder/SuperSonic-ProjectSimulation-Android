using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public GameObject UpgradeScreen;
    public Button upgradePlayerButton;
    public Button upgradeAIButton;
    public int playerUpgradeAmount=5;
    public int aiUpgradeAmount=7;

    ProductInfo playerProductInfo;
    ProductInfo aiProductInfo;
    // Start is called before the first frame update
    void Start()
    {
        upgradePlayerButton.onClick.AddListener(OnUpgradePlayerButtonClicked);
        upgradeAIButton.onClick.AddListener(OnUpgradeAIButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            UpgradeScreen.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other){
        if(other.CompareTag("Player")){
            UpgradeScreen.SetActive(false);
        }
    }
    void OnUpgradePlayerButtonClicked()
    {
        // // Find player product info dynamically
        // playerProductInfo = FindObjectOfType<PlayerController>().GetComponentInChildren<ProductInfo>();
        // if (playerProductInfo != null)
        // {
        //     playerProductInfo.UpgradePlayerProductLimit(playerUpgradeAmount);
        // }
    }
    void OnUpgradeAIButtonClicked()
    {
        // // Find AI product info dynamically
        // aiProductInfo = FindObjectOfType<AIController>().GetComponentInChildren<ProductInfo>();
        // if (aiProductInfo != null)
        // {
        //     aiProductInfo.UpgradeWorkerProductLimit(aiUpgradeAmount);
        // }
    }
}
