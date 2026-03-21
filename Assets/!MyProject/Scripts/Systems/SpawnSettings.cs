using UnityEngine;

public class SpawnSettings : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private int maxMonsters = 10;
    [SerializeField] private float spawnInterval = 5f;

    [Header("Animal Names")]
    [SerializeField] private string[] possibleNames = { "Лев", "Тигр", "Медведь", "Волк", "Лиса", "Заяц", "Слон", "Жираф", "Зебра", "Олень" };

    [Header("Rarity Settings")]
    [SerializeField]
    private RarityData[] rarities = new RarityData[]
    {
        new RarityData { rarityName = "Common", multiplier = 1f, spawnChance = 50f, rarityColor = Color.white },
        new RarityData { rarityName = "Uncommon", multiplier = 1.2f, spawnChance = 25f, rarityColor = Color.green },
        new RarityData { rarityName = "Rare", multiplier = 1.5f, spawnChance = 12f, rarityColor = Color.blue },
        new RarityData { rarityName = "Epic", multiplier = 2f, spawnChance = 8f, rarityColor = Color.magenta },
        new RarityData { rarityName = "Legendary", multiplier = 3f, spawnChance = 4f, rarityColor = new Color(1f, 0.5f, 0f) },
        new RarityData { rarityName = "Nightmare", multiplier = 5f, spawnChance = 1f, rarityColor = Color.red }
    };

    private float spawnTimer = 0f;
    private int currentMonsters = 0;

    void Start()
    {
        if (spawnArea == null)
            spawnArea = GetComponent<BoxCollider>();

        spawnTimer = spawnInterval;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && currentMonsters < maxMonsters)
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

        currentMonsters++;
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
        currentMonsters--;
    }
}