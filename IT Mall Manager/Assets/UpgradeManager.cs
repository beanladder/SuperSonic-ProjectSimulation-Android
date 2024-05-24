using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public Button upgradePlayerButton;
    public Button upgradeAIButton;
    public int playerUpgradeAmount = 5;
    public int aiUpgradeAmount = 7;
    public GameObject UpgradeScreen;
    void Start()
    {
        upgradePlayerButton.onClick.AddListener(OnUpgradePlayerButtonClicked);
        upgradeAIButton.onClick.AddListener(OnUpgradeAIButtonClicked);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UpgradeScreen.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UpgradeScreen.SetActive(false);
        }
    }
    void OnUpgradePlayerButtonClicked()
    {
        // Assuming SuperContainer.Instance.heldPackage is currently held by player
        ProductInfo productInfo = SuperContainer.Instance.heldPackage?.GetComponent<ProductInfo>();
        if (productInfo != null && !productInfo.isAI)
        {
            productInfo.UpgradePlayerProductLimit(productInfo.playerMaxProducts + playerUpgradeAmount);
        }
    }

    void OnUpgradeAIButtonClicked()
    {
        // Assuming SuperContainer.Instance.heldPackage is currently held by AI
        ProductInfo productInfo = SuperContainer.Instance.heldPackage?.GetComponent<ProductInfo>();
        if (productInfo != null && productInfo.isAI)
        {
            productInfo.UpgradeWorkerProductLimit(productInfo.aiMaxProducts + aiUpgradeAmount);
        }
    }
}
