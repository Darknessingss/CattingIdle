using TMPro;
using UnityEngine;

public class TameableAnimal : MonoBehaviour
{
    private bool isTamed = false;
    private bool isBeingRemoved = false;
    private float requiredFood;
    private MonsterSpawner monsterIncome;
    private RarityData rarity;
    private float currentFoodSpent = 0f;
    private AnimalInventory animalInventory;
    private AnimalLimitManager limitManager;

    public float RequiredFood => requiredFood;
    public bool IsTamed => isTamed;
    public float CurrentFoodSpent => currentFoodSpent;
    public float RemainingFood => requiredFood - currentFoodSpent;

    public void Initialize(RarityData rarityRef, AnimalInventory inventoryRef)
    {
        rarity = rarityRef;
        requiredFood = rarity.requiredFood;
        currentFoodSpent = 0f;
        animalInventory = inventoryRef;
        limitManager = FindFirstObjectByType<AnimalLimitManager>();
    }

    void Start()
    {
        monsterIncome = GetComponent<MonsterSpawner>();

        if (monsterIncome != null)
        {
            monsterIncome.enabled = false;
        }
    }

    public void AddFood(float amount)
    {
        if (isTamed) return;

        float remainingNeeded = requiredFood - currentFoodSpent;
        float foodToAdd = Mathf.Min(amount, remainingNeeded);

        currentFoodSpent += foodToAdd;
    }

    public void CompleteTaming()
    {
        if (currentFoodSpent >= requiredFood && !isTamed)
        {
            if (limitManager != null && !limitManager.CanTame)
            {
                Debug.Log($"Нельзя приручить! Лимит: {limitManager.CurrentTamedCount}/{limitManager.MaxTamedAnimals}");
                return;
            }

            isTamed = true;

            if (monsterIncome != null)
            {
                monsterIncome.enabled = true;
            }
            if (animalInventory != null)
            {
                animalInventory.TryAddAnimal(this);
            }

            if (limitManager != null)
            {
                limitManager.TryAddTamedAnimal();
            }

            SpawnSettings spawnSettings = FindFirstObjectByType<SpawnSettings>();
            if (spawnSettings != null)
            {
                spawnSettings.RemoveMonster();
            }

            Debug.Log("Животное приручено!");
        }
    }

    public float GetRemainingFood()
    {
        return requiredFood - currentFoodSpent;
    }

    public void MarkAsBeingRemoved()
    {
        isBeingRemoved = true;
    }

    void OnDestroy()
    {
        if (isTamed && limitManager != null && !isBeingRemoved)
        {
            limitManager.RemoveTamedAnimal();
        }
    }
}