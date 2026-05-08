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

    public int RequiredFood => requiredFood;
    public bool IsTamed => isTamed;
    public int RemainingFood => requiredFood - currentFoodSpent;

    public void Initialize(RarityData rarityRef, AnimalInventory inventoryRef, AnimalLimitManager limit)
    {
        requiredFood = rarityRef.requiredFood;
        currentFoodSpent = 0;
        animalInventory = inventoryRef;
        limitManager = limit;
    }

    void Start()
    {
        monsterIncome = GetComponent<MonsterSpawner>();
        if (monsterIncome != null)
            monsterIncome.enabled = false;
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
            animalInventory.TryAddAnimal(this);
            limitManager.TryAddTamedAnimal();
            FindFirstObjectByType<SpawnSettings>().RemoveMonster();
            Debug.Log("Животное приручено!");
        }
    }

    public int GetRemainingFood() => requiredFood - currentFoodSpent;
    public void MarkAsBeingRemoved() => isBeingRemoved = true;

    void OnDestroy()
    {
        if (isTamed && limitManager != null && !isBeingRemoved)
            limitManager.RemoveTamedAnimal();
    }
}