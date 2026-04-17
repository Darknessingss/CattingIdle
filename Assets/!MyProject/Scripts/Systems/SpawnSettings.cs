using UnityEngine;

public class SpawnSettings : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private int maxSpawnedMonsters = 10;
    [SerializeField] private float spawnInterval = 5f;

    [Header("Animal Names")]
    [SerializeField] private string[] possibleNames = { "Лев", "Тигр", "Медведь", "Волк", "Лиса", "Заяц", "Слон", "Жираф", "Зебра", "Олень" };

    [Header("Rarity Settings")]
    [SerializeField]
    private RarityData[] rarities = new RarityData[]
    {
        new RarityData { rarityName = "Common", minMultiplier = 1f, maxMultiplier = 1.2f, spawnChance = 50f, rarityColor = Color.white, requiredFood = 10f },
        new RarityData { rarityName = "Uncommon", minMultiplier = 1.2f, maxMultiplier = 2f, spawnChance = 25f, rarityColor = Color.green, requiredFood = 20f },
        new RarityData { rarityName = "Rare", minMultiplier = 1.5f, maxMultiplier = 3f, spawnChance = 12f, rarityColor = Color.blue, requiredFood = 35f },
        new RarityData { rarityName = "Epic", minMultiplier = 2f, maxMultiplier = 3.5f, spawnChance = 8f, rarityColor = Color.magenta, requiredFood = 50f },
        new RarityData { rarityName = "Legendary", minMultiplier = 3f, maxMultiplier = 4.5f, spawnChance = 4f, rarityColor = new Color(1f, 0.5f, 0f), requiredFood = 75f },
        new RarityData { rarityName = "Nightmare", minMultiplier = 5f, maxMultiplier = 6f, spawnChance = 1f, rarityColor = Color.red, requiredFood = 100f }
    };

    private float spawnTimer = 0f;
    private int currentSpawnedMonsters = 0;
    private AnimalLimitManager limitManager;
    private AnimalInventory animalInventory;

    void Start()
    {
        if (spawnArea == null)
            spawnArea = GetComponent<BoxCollider>();

        spawnTimer = spawnInterval;
        limitManager = FindFirstObjectByType<AnimalLimitManager>();
        animalInventory = FindFirstObjectByType<AnimalInventory>();
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
        if (monsterPrefab == null || spawnArea == null)
            return;

        RarityData selectedRarity = GetRandomRarity();
        Vector3 spawnPosition = GetRandomPositionInBox();
        GameObject newMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);

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
        tameable.Initialize(selectedRarity, animalInventory);

        currentSpawnedMonsters++;
    }

    private RarityData GetRandomRarity()
    {
        float totalChance = 0f;
        foreach (var rarity in rarities)
        {
            totalChance += rarity.spawnChance;
        }

        float randomValue = Random.Range(0f, totalChance);
        float currentChance = 0f;

        foreach (var rarity in rarities)
        {
            currentChance += rarity.spawnChance;
            if (randomValue <= currentChance)
            {
                return rarity;
            }
        }

        return rarities[0];
    }

    private Vector3 GetRandomPositionInBox()
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }

    public void RemoveMonster()
    {
        currentSpawnedMonsters--;
    }
}