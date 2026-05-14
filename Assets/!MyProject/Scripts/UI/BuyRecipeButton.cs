using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyRecipeButton : MonoBehaviour
{
    [SerializeField] private ItemSO item;
    [SerializeField] private int amount = 1;
    [SerializeField] private int price = 50;
    [SerializeField] private CraftingSystem craftingSystem;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;

    private Wallet wallet;

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();

        if (buyButton != null)
            buyButton.onClick.AddListener(BuyItem);

        if (priceText != null)
            priceText.text = $"{price}";
    }

    private void BuyItem()
    {
        if (wallet == null || craftingSystem == null) return;

        if (wallet.SpendMoney(price))
        {
            craftingSystem.AddItem(item, amount);
            Debug.Log($"Bought {amount}x {item.itemName} for {price}");
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
}