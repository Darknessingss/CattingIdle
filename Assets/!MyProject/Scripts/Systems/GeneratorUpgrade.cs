using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUpgrade : MonoBehaviour
{
    [SerializeField] private int maxUpgradeLevel = 5;
    [SerializeField] private int upgradeBasePrice = 100;
    [SerializeField] private float upgradePriceMultiplier = 1.8f;
    [SerializeField] private float baseFoodPerTick = 5f;
    [SerializeField] private float foodIncreasePerUpgrade = 2f;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI upgradePriceText;
    [SerializeField] private TextMeshProUGUI upgradeLevelText;
    [SerializeField] private TextMeshProUGUI bonusText;

    private Wallet wallet;
    private int currentUpgradeLevel;

    public int CurrentUpgradePrice => Mathf.RoundToInt(upgradeBasePrice * Mathf.Pow(upgradePriceMultiplier, currentUpgradeLevel));
    public float CurrentFoodPerTick => baseFoodPerTick + (currentUpgradeLevel * foodIncreasePerUpgrade);
    public event Action OnUpgradeChanged;

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();
        upgradeButton.onClick.AddListener(TryUpgrade);
        UpdateUI();
        RarityLocalizer.OnLanguageChanged += UpdateUI;
    }

    private void TryUpgrade()
    {
        if (currentUpgradeLevel >= maxUpgradeLevel) return;

        int price = CurrentUpgradePrice;
        if (!wallet.SpendMoney(price)) return;

        currentUpgradeLevel++;
        OnUpgradeChanged?.Invoke();
        UpdateUI();
        UpdateAllGenerators();
    }

    private void UpdateAllGenerators()
    {
        foreach (var generator in FindObjectsByType<FoodGenerator>(FindObjectsSortMode.None))
            generator.UpdateFoodAmount(CurrentFoodPerTick);
    }

    private void UpdateUI()
    {
        bool isMax = currentUpgradeLevel >= maxUpgradeLevel;

        if (upgradePriceText != null)
            upgradePriceText.text = isMax ? "MAX" : $"{RarityLocalizer.GetLocalizedCost()} {CurrentUpgradePrice}";

        if (upgradeLevelText != null)
            upgradeLevelText.text = $"{currentUpgradeLevel}/{maxUpgradeLevel}";

        if (bonusText != null)
            bonusText.text = currentUpgradeLevel == 0 ? "+0" : $"+{CurrentFoodPerTick - baseFoodPerTick:F0}";

        if (upgradeButton != null)
            upgradeButton.interactable = !isMax;
    }

    public float GetCurrentFoodPerTick() => CurrentFoodPerTick;

    void OnDestroy()
    {
        RarityLocalizer.OnLanguageChanged -= UpdateUI;
    }
}