using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyLimitButton : MonoBehaviour
{
    [Header("Upgrade Settings")]
    [SerializeField] private int limitIncrease = 2;
    [SerializeField] private int maxUpgradeLevel = 5;
    [SerializeField] private int basePrice = 500;
    [SerializeField] private float priceMultiplier = 1.6f;
    [SerializeField] private int maxTotalLimit = 20;

    [Header("UI References")]
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI limitText;

    private AnimalLimitManager limitManager;
    private Wallet wallet;
    private int currentUpgradeLevel = 0;

    public int CurrentPrice => Mathf.RoundToInt(basePrice * Mathf.Pow(priceMultiplier, currentUpgradeLevel));

    void Start()
    {
        limitManager = FindFirstObjectByType<AnimalLimitManager>();
        wallet = FindFirstObjectByType<Wallet>();

        if (buyButton != null)
            buyButton.onClick.AddListener(BuyLimit);

        UpdateUI();
    }

    private void BuyLimit()
    {
        if (limitManager == null || wallet == null) return;

        if (currentUpgradeLevel >= maxUpgradeLevel)
        {
            Debug.Log("Достигнут максимальный уровень улучшения лимита!");
            return;
        }

        if (limitManager.MaxTamedAnimals >= maxTotalLimit)
        {
            Debug.Log($"Достигнут максимальный лимит! Максимум: {maxTotalLimit}");
            return;
        }

        int price = CurrentPrice;

        if (wallet.SpendMoney(price))
        {
            currentUpgradeLevel++;
            limitManager.IncreaseMaxLimit(limitIncrease);
            UpdateUI();
            Debug.Log($"Лимит увеличен на {limitIncrease}! Теперь максимум: {limitManager.MaxTamedAnimals}. Следующая цена: {CurrentPrice}");
        }
        else
        {
            Debug.Log($"Недостаточно монет! Нужно: {price}");
        }
    }

    private void UpdateUI()
    {
        if (priceText != null)
        {
            if (currentUpgradeLevel >= maxUpgradeLevel)
                priceText.text = "MAX";
            else
                priceText.text = $"Цена: {CurrentPrice}";
        }

        if (limitText != null && limitManager != null)
            limitText.text = $"+{limitIncrease}";

        if (buyButton != null)
            buyButton.interactable = currentUpgradeLevel < maxUpgradeLevel;
    }
}