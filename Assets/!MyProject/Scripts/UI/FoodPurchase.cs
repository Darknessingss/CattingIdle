using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FoodPurchase : MonoBehaviour
{
    [Header("Generator Purchase Settings")]
    [SerializeField] private int generatorBasePrice = 100;
    [SerializeField] private float generatorPriceMultiplier = 1.5f;

    [Header("References")]
    [SerializeField] private GameObject generatorPrefab;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;

    private Wallet wallet;
    private FoodSystem foodSystem;
    private GeneratorLimitManager limitManager;
    private GeneratorUpgrade generatorUpgrade;
    private List<GameObject> spawnedGenerators = new List<GameObject>();
    private int generatorsPurchased = 0;

    public int CurrentGeneratorPrice => Mathf.RoundToInt(generatorBasePrice * Mathf.Pow(generatorPriceMultiplier, generatorsPurchased));

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();
        foodSystem = FindFirstObjectByType<FoodSystem>();
        limitManager = FindFirstObjectByType<GeneratorLimitManager>();
        generatorUpgrade = FindFirstObjectByType<GeneratorUpgrade>();

        if (buyButton != null)
            buyButton.onClick.AddListener(BuyGenerator);

        UpdateUI();
    }

    private void BuyGenerator()
    {
        if (limitManager == null || !limitManager.CanBuy)
        {
            Debug.Log($"Достигнут лимит генераторов! Максимум: {limitManager?.MaxGenerators}");
            return;
        }

        int nextWaypointIndex = spawnedGenerators.Count;

        if (nextWaypointIndex >= waypoints.Count)
        {
            Debug.LogError("Недостаточно точек пути для нового генератора!");
            return;
        }

        if (wallet == null || foodSystem == null) return;

        int price = CurrentGeneratorPrice;

        if (wallet.SpendMoney(price))
        {
            generatorsPurchased++;
            limitManager.TryAddGenerator();
            SpawnGenerator(nextWaypointIndex);
            UpdateUI();
            Debug.Log($"Генератор куплен за {price}! Следующий будет стоить: {CurrentGeneratorPrice}");
        }
        else
        {
            Debug.Log($"Недостаточно монет! Нужно: {price}");
        }
    }

    private void SpawnGenerator(int waypointIndex)
    {
        if (generatorPrefab == null) return;

        Transform spawnPoint = waypoints[waypointIndex];
        GameObject newGenerator = Instantiate(generatorPrefab, spawnPoint.position, generatorPrefab.transform.rotation);
        spawnedGenerators.Add(newGenerator);

        FoodGenerator foodGenerator = newGenerator.GetComponent<FoodGenerator>();
        if (foodGenerator == null)
            foodGenerator = newGenerator.AddComponent<FoodGenerator>();

        float currentFood = generatorUpgrade != null ? generatorUpgrade.GetCurrentFoodPerTick() : 5f;
        float currentDelay = generatorUpgrade != null ? generatorUpgrade.CurrentTimeDelay : 10f;

        foodGenerator.Initialize(foodSystem, currentFood, currentDelay);
    }

    private void UpdateUI()
    {
        if (priceText != null)
            priceText.text = $"Цена: {CurrentGeneratorPrice}";

        if (buyButton != null && limitManager != null)
            buyButton.interactable = limitManager.CanBuy;
    }
}