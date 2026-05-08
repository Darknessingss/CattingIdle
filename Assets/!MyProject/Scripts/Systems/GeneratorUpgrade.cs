using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUpgrade : MonoBehaviour
{
    [Header("Upgrade Settings")]
    [SerializeField] private int maxUpgradeLevel = 5;
    [SerializeField] private int upgradeBasePrice = 100;
    [SerializeField] private float upgradePriceMultiplier = 1.8f;
    [SerializeField] private float baseFoodPerTick = 5f;
    [SerializeField] private float foodIncreasePerUpgrade = 2f;
    [SerializeField] private float baseTimeDelay = 10f;

    [Header("UI References")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI upgradePriceText;
    [SerializeField] private TextMeshProUGUI upgradeLevelText;
    [SerializeField] private TextMeshProUGUI bonusText;

    private Wallet wallet;
    private int currentUpgradeLevel = 0;

    public int CurrentUpgradeLevel => currentUpgradeLevel;
    public int MaxUpgradeLevel => maxUpgradeLevel;
    public int CurrentUpgradePrice => Mathf.RoundToInt(upgradeBasePrice * Mathf.Pow(upgradePriceMultiplier, currentUpgradeLevel));
    public float CurrentFoodPerTick => baseFoodPerTick + (currentUpgradeLevel * foodIncreasePerUpgrade);
    public float CurrentTimeDelay => baseTimeDelay;
    public float BonusFood => currentUpgradeLevel * foodIncreasePerUpgrade;

    public event Action OnUpgradeChanged;

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(TryUpgrade);

        UpdateUI();
    }

    private void TryUpgrade()
    {
        if (currentUpgradeLevel >= maxUpgradeLevel)
        {
            Debug.Log("Достигнут максимальный уровень улучшения!");
            return;
        }

        if (wallet == null)
        {
            Debug.Log("Система кошелька не найдена!");
            return;
        }

        int price = CurrentUpgradePrice;

        if (wallet.SpendMoney(price))
        {
            currentUpgradeLevel++;
            OnUpgradeChanged?.Invoke();
            UpdateUI();
            UpdateAllGenerators();

            Debug.Log($"Генератор улучшен до уровня {currentUpgradeLevel}/{maxUpgradeLevel}! Теперь даёт +{CurrentFoodPerTick} еды");
        }
        else
        {
            Debug.Log($"Недостаточно монет! Нужно: {price}");
        }
    }

    private void UpdateAllGenerators()
    {
        FoodGenerator[] generators = FindObjectsByType<FoodGenerator>(FindObjectsSortMode.None);
        foreach (FoodGenerator generator in generators)
        {
            generator.UpdateFoodAmount(CurrentFoodPerTick);
        }
    }

    private void UpdateUI()
    {
        if (upgradePriceText != null)
            upgradePriceText.text = $"Цена: {CurrentUpgradePrice}";

        if (upgradeLevelText != null)
        {
            upgradeLevelText.text = $"{currentUpgradeLevel}/{maxUpgradeLevel}";
            upgradeLevelText.color = currentUpgradeLevel >= maxUpgradeLevel ? Color.red : Color.black;
        }

        if (bonusText != null)
            bonusText.text = currentUpgradeLevel == 0 ? "+0" : $"+{BonusFood}";

        if (upgradeButton != null)
            upgradeButton.interactable = currentUpgradeLevel < maxUpgradeLevel;
    }

    public float GetCurrentFoodPerTick() => CurrentFoodPerTick;
}