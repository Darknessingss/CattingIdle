using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodPurchase : MonoBehaviour
{
    [Header("Purchase Settings")]
    [SerializeField] private int foodAmount = 10;
    [SerializeField] private int foodPrice = 30;

    [Header("UI References")]
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI foodAmountText;

    private Wallet wallet;
    private FoodSystem foodSystem;

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();
        foodSystem = FindFirstObjectByType<FoodSystem>();

        if (buyButton != null)
            buyButton.onClick.AddListener(BuyFood);

        if (priceText != null)
            priceText.text = $"Цена: {foodPrice}";

        if (foodAmountText != null)
            foodAmountText.text = $"+{foodAmount}";
    }

    private void BuyFood()
    {
        if (wallet != null && foodSystem != null)
        {
            if (wallet.SpendMoney(foodPrice))
            {
                foodSystem.AddFood(foodAmount);
                Debug.Log($"Куплено {foodAmount} еды за {foodPrice} монет!");
            }
            else
            {
                Debug.Log("Недостаточно монет!");
            }
        }
    }
}