using UnityEngine;

public class FoodGenerator : MonoBehaviour
{
    private FoodSystem foodSystem;
    private float foodPerTick;
    private float timeDelay;
    private float timer;
    private bool isInitialized = false;

    public void Initialize(FoodSystem system, float foodAmount, float delay)
    {
        foodSystem = system;
        foodPerTick = foodAmount;
        timeDelay = delay;
        timer = delay;
        isInitialized = true;
    }

    public void UpdateFoodAmount(float newFoodAmount)
    {
        foodPerTick = newFoodAmount;
        Debug.Log($"Генератор теперь даёт +{foodPerTick} еды");
    }

    void Update()
    {
        if (!isInitialized) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (foodSystem != null)
            {
                foodSystem.AddFood(foodPerTick);
                Debug.Log($"Генератор выдал {foodPerTick} еды!");
            }
            timer = timeDelay;
        }
    }
}
