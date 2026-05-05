using TMPro;
using UnityEngine;

public class FoodSystem : MonoBehaviour
{
    [SerializeField] private float startingFood = 100f;
    [SerializeField] private TextMeshProUGUI foodText;

    private float currentFood;

    void Start()
    {
        currentFood = startingFood;
        UpdateFoodUI();
    }

    public float GetCurrentFood() => currentFood;

    public void AddFood(float amount)
    {
        currentFood += amount;
        UpdateFoodUI();
    }

    public bool ConsumeFood(float amount)
    {
        if (currentFood >= amount)
        {
            currentFood -= amount;
            UpdateFoodUI();
            return true;
        }
        return false;
    }

    private void UpdateFoodUI()
    {
        if (foodText != null)
            foodText.text = $"{Mathf.RoundToInt(currentFood)}";
    }
}