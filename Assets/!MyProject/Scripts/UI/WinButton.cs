using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinButton : MonoBehaviour
{
    [Header("Win Requirements")]
    [SerializeField] private ItemSO requiredItem;
    [SerializeField] private float requiredMoney = 1000000f;
    [SerializeField] private GameObject winPanel;

    [Header("UI References")]
    [SerializeField] private Button winButton;
    [SerializeField] private TextMeshProUGUI costText;

    private CraftingSystem craftingSystem;
    private Wallet wallet;

    void Start()
    {
        craftingSystem = FindFirstObjectByType<CraftingSystem>();
        wallet = FindFirstObjectByType<Wallet>();

        if (winButton != null)
            winButton.onClick.AddListener(CheckWinCondition);

        if (winPanel != null)
            winPanel.SetActive(false);

        UpdateCostText();
    }

    private void CheckWinCondition()
    {
        if (craftingSystem == null || wallet == null) return;

        int currentItemCount = craftingSystem.GetItemCount(requiredItem);
        float currentMoney = wallet.WalletMoney;

        if (currentItemCount >= 1 && currentMoney >= requiredMoney)
        {
            craftingSystem.AddItem(requiredItem, -1);
            wallet.SpendMoney(requiredMoney);

            if (winPanel != null)
                winPanel.SetActive(true);

            Debug.Log("WIN! Game completed!");
        }
        else
        {
            Debug.Log($"COST: {requiredMoney} coins");
        }
    }

    private void UpdateCostText()
    {
        if (costText != null)
        {
            costText.text = $"COST: {requiredMoney}";
        }
    }
}