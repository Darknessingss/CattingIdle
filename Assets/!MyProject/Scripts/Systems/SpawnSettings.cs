using UnityEngine;

public class SpawnSettings : MonoBehaviour
{
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private int maxSpawnedMonsters = 10;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private string[] possibleNames = { "Lion", "Tiger", "Bear", "Wolf", "Fox", "Rabbit", "Elephant", "Giraffe", "Zebra", "Deer" };
    [SerializeField] private AnimalLimitManager _limitManager;
    [SerializeField] private AnimalInventory animalInventory;

    [SerializeField]
    private RarityData[] rarities = new RarityData[]
    {
        new RarityData { rarityName = "Common", minMultiplier = 1f, maxMultiplier = 1.2f, spawnChance = 70f, rarityColor = Color.white, minRequiredFood = 8, maxRequiredFood = 20 },
        new RarityData { rarityName = "Uncommon", minMultiplier = 1.2f, maxMultiplier = 1.8f, spawnChance = 35f, rarityColor = Color.green, minRequiredFood = 25, maxRequiredFood = 40 },
        new RarityData { rarityName = "Rare", minMultiplier = 1.8f, maxMultiplier = 3f, spawnChance = 15f, rarityColor = Color.blue, minRequiredFood = 45, maxRequiredFood = 100 },
        new RarityData { rarityName = "Epic", minMultiplier = 2.5f, maxMultiplier = 4f, spawnChance = 8f, rarityColor = Color.magenta, minRequiredFood = 75, maxRequiredFood = 150 },
        new RarityData { rarityName = "Legendary", minMultiplier = 3.5f, maxMultiplier = 5f, spawnChance = 3f, rarityColor = new Color(1f, 0.5f, 0f), minRequiredFood = 125, maxRequiredFood = 250 },
        new RarityData { rarityName = "Nightmare", minMultiplier = 6f, maxMultiplier = 10f, spawnChance = 0.1f, rarityColor = Color.red, minRequiredFood = 600, maxRequiredFood = 1000 }
    };

    private float[] originalChances;
    private float bonusChance = 0f;

    private float spawnTimer;
    private int currentSpawnedMonsters;

    void Start()
    {
        if (spawnArea == null)
            spawnArea = GetComponent<BoxCollider>();

        originalChances = new float[rarities.Length];
        for (int i = 0; i < rarities.Length; i++)
            originalChances[i] = rarities[i].spawnChance;

        spawnTimer = spawnInterval;
    }

    public void IncreaseRarityChance(float increasePercent)
    {
        bonusChance += increasePercent;

        for (int i = 0; i < rarities.Length; i++)
            rarities[i].spawnChance = originalChances[i];

        rarities[3].spawnChance += bonusChance;
        rarities[4].spawnChance += bonusChance;
        rarities[5].spawnChance += bonusChance;

        Debug.Log($"Шансы: Epic: {rarities[3].spawnChance}%, Legendary: {rarities[4].spawnChance}%, Nightmare: {rarities[5].spawnChance}%");
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && currentSpawnedMonsters < maxSpawnedMonsters)
        {
            SpawnMonster();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnMonster()
    {
        if (monsterPrefab == null || spawnArea == null) return;

        RarityData selectedRarity = GetRandomRarity();

        selectedRarity.requiredFood = Random.Range(selectedRarity.minRequiredFood, selectedRarity.maxRequiredFood + 1);

        GameObject newMonster = Instantiate(monsterPrefab, GetRandomPositionInBox(), monsterPrefab.transform.rotation);

        MonsterSpawner monsterIncome = newMonster.GetComponent<MonsterSpawner>();
        if (monsterIncome == null)
            monsterIncome = newMonster.AddComponent<MonsterSpawner>();
        monsterIncome.Initialize(selectedRarity);

        AnimalName animalName = newMonster.GetComponent<AnimalName>();
        if (animalName == null)
            animalName = newMonster.AddComponent<AnimalName>();
        animalName.GenerateRandomName(possibleNames);
        animalName.SetRarity(selectedRarity);

        TameableAnimal tameable = newMonster.GetComponent<TameableAnimal>();
        if (tameable == null)
            tameable = newMonster.AddComponent<TameableAnimal>();
        tameable.Initialize(selectedRarity, animalInventory, _limitManager);

        currentSpawnedMonsters++;
    }

    private RarityData GetRandomRarity()
    {
        float totalChance = 0f;
        foreach (var rarity in rarities)
            totalChance += rarity.spawnChance;

        float randomValue = Random.Range(0f, totalChance);
        float currentChance = 0f;

        foreach (var rarity in rarities)
        {
            currentChance += rarity.spawnChance;
            if (randomValue <= currentChance)
            {
                RarityData copy = new RarityData
                {
                    rarityName = rarity.rarityName,
                    minMultiplier = rarity.minMultiplier,
                    maxMultiplier = rarity.maxMultiplier,
                    spawnChance = rarity.spawnChance,
                    rarityColor = rarity.rarityColor,
                    minRequiredFood = rarity.minRequiredFood,
                    maxRequiredFood = rarity.maxRequiredFood
                };
                return copy;
            }
        }

        return rarities[0];
    }

    private Vector3 GetRandomPositionInBox()
    {
        Bounds bounds = spawnArea.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void RemoveMonster() => currentSpawnedMonsters--;
}