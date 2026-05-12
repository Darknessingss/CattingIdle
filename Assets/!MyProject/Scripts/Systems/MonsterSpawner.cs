using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private float baseIncome = 10f;
    [SerializeField] private float minIncomeInterval = 0.5f;
    [SerializeField] private float maxIncomeInterval = 5f;

    private float income;
    private float incomeInterval;
    private float incomeTimer;
    private float incomeMultiplier = 1f;
    private bool isNightmare;

    public void Initialize(RarityData rarity)
    {
        income = baseIncome * rarity.multiplier;
        incomeInterval = Random.Range(minIncomeInterval, maxIncomeInterval);
        incomeTimer = incomeInterval;
        isNightmare = rarity.rarityName == "Nightmare";

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = rarity.rarityColor;

        transform.localScale = Vector3.one * (30f + (rarity.multiplier / 10f));
    }

    public void ApplyIncomeMultiplier(float multiplier)
    {
        incomeMultiplier *= multiplier;
    }

    void Update()
    {
        incomeTimer -= Time.deltaTime;
        if (incomeTimer <= 0f)
        {
            Wallet wallet = FindFirstObjectByType<Wallet>();
            if (wallet != null)
                wallet.AddMoney(income * incomeMultiplier);
            incomeTimer = incomeInterval;
        }
    }

    public float GetIncome() => income * incomeMultiplier;
    public float GetIncomeInterval() => incomeInterval;
    public bool IsNightmare() => isNightmare;

    void OnDestroy()
    {
        SpawnSettings spawnSettings = FindFirstObjectByType<SpawnSettings>();
        if (spawnSettings != null)
            spawnSettings.RemoveMonster();
    }
}