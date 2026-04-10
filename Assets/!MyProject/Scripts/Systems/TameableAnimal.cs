using TMPro;
using UnityEngine;

public class TameableAnimal : MonoBehaviour
{
    private bool isTamed = false;
    private float requiredFood;
    private MonsterSpawner monsterIncome;
    private RarityData rarity;
    private float currentFoodSpent = 0f;

    public float RequiredFood => requiredFood;
    public bool IsTamed => isTamed;
    public float CurrentFoodSpent => currentFoodSpent;
    public float RemainingFood => requiredFood - currentFoodSpent;

    public void Initialize(RarityData rarityRef)
    {
        rarity = rarityRef;
        requiredFood = rarity.requiredFood;
        currentFoodSpent = 0f;
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
        if (currentFoodSpent >= requiredFood)
        {
            isTamed = true;

            if (monsterIncome != null)
            {
                monsterIncome.enabled = true;
            }

            Debug.Log("Животное приручено!");
        }
    }

    public float GetRemainingFood()
    {
        return requiredFood - currentFoodSpent;
    }
}