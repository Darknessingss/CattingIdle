using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FoodPurchase : MonoBehaviour
{
    [SerializeField] private int generatorBasePrice = 100;
    [SerializeField] private float generatorPriceMultiplier = 1.5f;
    [SerializeField] private GameObject generatorPrefab;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;

    private Wallet wallet;
    private FoodSystem foodSystem;
    private GeneratorLimitManager limitManager;
    private GeneratorUpgrade generatorUpgrade;
    private List<GameObject> spawnedGenerators = new List<GameObject>();

    public int CurrentGeneratorPrice => Mathf.RoundToInt(generatorBasePrice * Mathf.Pow(generatorPriceMultiplier, spawnedGenerators.Count));

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();
        foodSystem = FindFirstObjectByType<FoodSystem>();
        limitManager = FindFirstObjectByType<GeneratorLimitManager>();
        generatorUpgrade = FindFirstObjectByType<GeneratorUpgrade>();

        buyButton.onClick.AddListener(BuyGenerator);
        UpdateUI();
    }

    private void BuyGenerator()
    {
        if (!limitManager.CanBuy) return;
        if (spawnedGenerators.Count >= waypoints.Count) return;

        int price = CurrentGeneratorPrice;
        if (!wallet.SpendMoney(price)) return;

        SpawnGenerator(spawnedGenerators.Count);
        limitManager.TryAddGenerator();
        UpdateUI();
    }

    private void SpawnGenerator(int waypointIndex)
    {
        Transform spawnPoint = waypoints[waypointIndex];
        GameObject newGenerator = Instantiate(generatorPrefab, spawnPoint.position, generatorPrefab.transform.rotation);
        spawnedGenerators.Add(newGenerator);

        FoodGenerator foodGenerator = newGenerator.GetComponent<FoodGenerator>();
        if (foodGenerator == null)
            foodGenerator = newGenerator.AddComponent<FoodGenerator>();

        float currentFood = generatorUpgrade != null ? generatorUpgrade.GetCurrentFoodPerTick() : 5f;
        foodGenerator.Initialize(foodSystem, currentFood, 10f);
    }

    private void UpdateUI()
    {
        bool isMax = !limitManager.CanBuy || spawnedGenerators.Count >= waypoints.Count;

        if (priceText != null)
            priceText.text = isMax ? "MAX" : $"COST: {CurrentGeneratorPrice}";

        if (buyButton != null)
            buyButton.interactable = !isMax;
    }
}