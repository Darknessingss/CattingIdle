using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldenFoxUpgrade : MonoBehaviour
{
    [Header("Upgrade Settings")]
    [SerializeField] private int maxUpgradeLevel = 10;
    [SerializeField] private int basePrice = 1000;
    [SerializeField] private float priceMultiplier = 1.8f;
    [SerializeField] private float chanceIncreasePercent = 0.05f;

    [Header("UI References")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI effectText;

    private Wallet wallet;
    private int currentUpgradeLevel;
    private SpawnSettings spawnSettings;

    public int CurrentPrice => Mathf.RoundToInt(basePrice * Mathf.Pow(priceMultiplier, currentUpgradeLevel));
    public float TotalChanceIncrease => currentUpgradeLevel * chanceIncreasePercent;

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();
        spawnSettings = FindFirstObjectByType<SpawnSettings>();

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(TryUpgrade);

        UpdateUI();
        RarityLocalizer.OnLanguageChanged += UpdateUI;
    }

    private void TryUpgrade()
    {
        if (currentUpgradeLevel >= maxUpgradeLevel)
        {
            Debug.Log("Максимальный уровень достигнут!");
            return;
        }

        int price = CurrentPrice;
        if (!wallet.SpendMoney(price)) return;

        currentUpgradeLevel++;
        ApplyUpgrade();
        UpdateUI();
    }

    private void ApplyUpgrade()
    {
        if (spawnSettings != null)
            spawnSettings.IncreaseGoldenFoxChance(TotalChanceIncrease);
    }

    private void UpdateUI()
    {
        bool isMax = currentUpgradeLevel >= maxUpgradeLevel;

        if (priceText != null)
            priceText.text = isMax ? "MAX" : $"{RarityLocalizer.GetLocalizedCost()} {CurrentPrice}";

        if (levelText != null)
            levelText.text = $"{currentUpgradeLevel}/{maxUpgradeLevel}";

        if (effectText != null)
            effectText.text = $"+{TotalChanceIncrease}%";

        if (upgradeButton != null)
            upgradeButton.interactable = !isMax;
    }

    void OnDestroy()
    {
        RarityLocalizer.OnLanguageChanged -= UpdateUI;
    }
}