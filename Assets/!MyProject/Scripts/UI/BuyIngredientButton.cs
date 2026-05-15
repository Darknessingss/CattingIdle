using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyIngredientButton : MonoBehaviour
{
    [SerializeField] private ItemSO item;
    [SerializeField] private int price = 50;
    [SerializeField] private int amount = 1;
    [SerializeField] private CraftingSystem craftingSystem;
    [SerializeField] private TextMeshProUGUI priceText;

    private Wallet wallet;

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();

        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(BuyItem);

        if (priceText != null)
            priceText.text = $"COST: {price}";
    }

    private void BuyItem()
    {
        if (wallet == null || craftingSystem == null)
        {
            Debug.Log("Wallet or CraftingSystem not found!");
            return;
        }

        if (wallet.SpendMoney(price))
        {
            craftingSystem.AddItem(item, amount);
            Debug.Log($"Bought {amount}x {item.itemName} for {price} coins");
        }
        else
        {
            Debug.Log($"Not enough money! Need {price} coins");
        }
    }
}