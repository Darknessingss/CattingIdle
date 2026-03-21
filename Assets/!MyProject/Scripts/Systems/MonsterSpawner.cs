using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Income Settings")]
    [SerializeField] private float baseIncome = 10f;
    [SerializeField] private float minIncomeInterval = 0.5f;
    [SerializeField] private float maxIncomeInterval = 5f;

    private RarityData rarity;
    private float income;
    private float incomeInterval;
    private float incomeTimer = 0f;

    public void Initialize(RarityData rarityRef)
    {
        rarity = rarityRef;

        income = baseIncome * rarity.multiplier;
        incomeInterval = Random.Range(minIncomeInterval, maxIncomeInterval);
        incomeTimer = incomeInterval;

        ApplyVisuals();
    }

    void Update()
    {
        incomeTimer -= Time.deltaTime;

        if (incomeTimer <= 0f)
        {
            AddIncome();
            incomeTimer = incomeInterval;
        }
    }

    private void AddIncome()
    {
        Wallet wallet = FindFirstObjectByType<Wallet>();
        if (wallet != null)
        {
            wallet.AddMoney(income);
        }
    }

    private void ApplyVisuals()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && rarity != null)
        {
            renderer.material.color = rarity.rarityColor;
        }

        transform.localScale = Vector3.one * (0.8f + (rarity.multiplier / 10f));
    }

    public float GetIncome()
    {
        return income;
    }

    public float GetIncomeInterval()
    {
        return incomeInterval;
    }

    void OnDestroy()
    {
        SpawnSettings spawnSettings = FindFirstObjectByType<SpawnSettings>();
        if (spawnSettings != null)
        {
            spawnSettings.RemoveMonster();
        }
    }
}