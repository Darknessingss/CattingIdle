using UnityEngine;

public class TameableAnimal : MonoBehaviour
{
    private bool isTamed;
    private bool isBeingRemoved;
    private float requiredFood;
    private MonsterSpawner monsterIncome;
    private float currentFoodSpent;
    private AnimalInventory animalInventory;
    private AnimalLimitManager limitManager;

    public float RequiredFood => requiredFood;
    public bool IsTamed => isTamed;
    public float RemainingFood => requiredFood - currentFoodSpent;

    public void Initialize(RarityData rarityRef, AnimalInventory inventoryRef)
    {
        requiredFood = rarityRef.requiredFood;
        currentFoodSpent = 0f;
        animalInventory = inventoryRef;
        limitManager = FindFirstObjectByType<AnimalLimitManager>();
    }

    void Start()
    {
        monsterIncome = GetComponent<MonsterSpawner>();
        if (monsterIncome != null)
            monsterIncome.enabled = false;
    }

    public void AddFood(float amount)
    {
        if (isTamed) return;
        float remainingNeeded = requiredFood - currentFoodSpent;
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

    public float GetRemainingFood() => requiredFood - currentFoodSpent;
    public void MarkAsBeingRemoved() => isBeingRemoved = true;

    void OnDestroy()
    {
        if (isTamed && limitManager != null && !isBeingRemoved)
            limitManager.RemoveTamedAnimal();
    }
}