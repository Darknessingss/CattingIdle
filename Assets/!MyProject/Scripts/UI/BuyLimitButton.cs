using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyLimitButton : MonoBehaviour
{
    [SerializeField] private int limitIncrease = 2;
    [SerializeField] private int maxUpgradeLevel = 5;
    [SerializeField] private int basePrice = 500;
    [SerializeField] private float priceMultiplier = 1.6f;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI limitText;

    private AnimalLimitManager limitManager;
    private Wallet wallet;
    private int currentUpgradeLevel;

    public int CurrentPrice => Mathf.RoundToInt(basePrice * Mathf.Pow(priceMultiplier, currentUpgradeLevel));

    void Start()
    {
        limitManager = FindFirstObjectByType<AnimalLimitManager>();
        wallet = FindFirstObjectByType<Wallet>();
        buyButton.onClick.AddListener(BuyLimit);
        UpdateUI();
    }

    private void BuyLimit()
    {
        if (currentUpgradeLevel >= maxUpgradeLevel) return;
        if (limitManager.MaxTamedAnimals >= 20) return;

        int price = CurrentPrice;
        if (!wallet.SpendMoney(price)) return;

        currentUpgradeLevel++;
        limitManager.IncreaseMaxLimit(limitIncrease);
        UpdateUI();
    }

    private void UpdateUI()
    {
        bool isMax = currentUpgradeLevel >= maxUpgradeLevel;

        if (priceText != null)
            priceText.text = isMax ? "MAX" : $"COST: {CurrentPrice}";

        if (limitText != null)
            limitText.text = $"+{limitIncrease}";

        if (buyButton != null)
            buyButton.interactable = !isMax;
    }
}