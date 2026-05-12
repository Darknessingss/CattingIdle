using UnityEngine;

public class TameableAnimal : MonoBehaviour
{
    private bool isTamed;
    private bool isBeingRemoved;
    private int requiredFood;
    private MonsterSpawner monsterIncome;
    private int currentFoodSpent;
    private AnimalInventory animalInventory;
    private AnimalLimitManager limitManager;
    private bool isNightmare;
    private NightmareTimer nightmareTimer;

    public int RequiredFood => requiredFood;
    public bool IsTamed => isTamed;
    public int RemainingFood => requiredFood - currentFoodSpent;

    public void Initialize(RarityData rarityRef, AnimalInventory inventoryRef, AnimalLimitManager limit)
    {
        requiredFood = rarityRef.requiredFood;
        currentFoodSpent = 0;
        animalInventory = inventoryRef;
        limitManager = limit;
        isNightmare = rarityRef.rarityName == "Nightmare";
    }

    void Start()
    {
        monsterIncome = GetComponent<MonsterSpawner>();
        if (monsterIncome != null)
            monsterIncome.enabled = false;

        if (isNightmare)
        {
            nightmareTimer = GetComponent<NightmareTimer>();
            if (nightmareTimer == null)
                nightmareTimer = gameObject.AddComponent<NightmareTimer>();
        }
    }

    public void AddFood(int amount)
    {
        if (isTamed) return;
        int remainingNeeded = requiredFood - currentFoodSpent;
        currentFoodSpent += Mathf.Min(amount, remainingNeeded);
    }

    public void CompleteTaming()
    {
        if (currentFoodSpent >= requiredFood && !isTamed)
        {
            if (!limitManager.CanTame)
            {
                Debug.Log($"Лимит: {limitManager.CurrentTamedCount}/{limitManager.MaxTamedAnimals}");
                return;
            }

            isTamed = true;
            monsterIncome.enabled = true;

            if (isNightmare)
            {
                ApplyNightmarePenalty();
                if (nightmareTimer != null)
                    nightmareTimer.StopTimer();
            }

            animalInventory.TryAddAnimal(this);
            limitManager.TryAddTamedAnimal();
            FindFirstObjectByType<SpawnSettings>().RemoveMonster();
            Debug.Log("Животное приручено!");
        }
    }

    private void ApplyNightmarePenalty()
    {
        TameableAnimal[] allAnimals = FindObjectsByType<TameableAnimal>(FindObjectsSortMode.None);
        foreach (TameableAnimal animal in allAnimals)
        {
            if (animal.isTamed && animal != this)
            {
                MonsterSpawner spawner = animal.GetComponent<MonsterSpawner>();
                if (spawner != null)
                    spawner.ApplyIncomeMultiplier(0.5f);
            }
        }

        if (monsterIncome != null)
            monsterIncome.ApplyIncomeMultiplier(1.5f);
    }

    public int GetRemainingFood() => requiredFood - currentFoodSpent;
    public void MarkAsBeingRemoved() => isBeingRemoved = true;

    void OnDestroy()
    {
        if (isTamed && limitManager != null && !isBeingRemoved)
            limitManager.RemoveTamedAnimal();
    }
}