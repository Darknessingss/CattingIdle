using System;
using TMPro;
using UnityEngine;

public class AnimalLimitManager : MonoBehaviour
{
    [Header("Limits Settings")]
    [SerializeField] private int maxTamedAnimals = 10;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tamedCountText;

    private int currentTamedCount = 0;

    public int MaxTamedAnimals => maxTamedAnimals;
    public int CurrentTamedCount => currentTamedCount;
    public bool CanTame => currentTamedCount < maxTamedAnimals;

    public event Action OnTamedCountChanged;

    void Start()
    {
        UpdateUI();
    }

    public bool TryAddTamedAnimal()
    {

        if (currentTamedCount < maxTamedAnimals)
        {
            currentTamedCount++;
            OnTamedCountChanged?.Invoke();
            UpdateUI();
            Debug.Log($"Приручено животных: {currentTamedCount}/{maxTamedAnimals}");
            return true;
        }
        else
        {
            Debug.Log($"Достигнут лимит прирученных животных! Максимум: {maxTamedAnimals}");
            return false;
        }
    }

    public void RemoveTamedAnimal()
    {
        if (currentTamedCount > 0)
        {
            currentTamedCount--;
            OnTamedCountChanged?.Invoke();
            UpdateUI();
            Debug.Log($"Приручено животных: {currentTamedCount}/{maxTamedAnimals}");
        }
    }

    public void IncreaseMaxLimit(int amount)
    {
        maxTamedAnimals += amount;
        UpdateUI();
        Debug.Log($"Максимальный лимит увеличен до {maxTamedAnimals}");
    }

    private void UpdateUI()
    {
        if (tamedCountText != null)
        {
            tamedCountText.text = $"{currentTamedCount}/{maxTamedAnimals}";

            if (currentTamedCount >= maxTamedAnimals)
                tamedCountText.color = Color.red;
            else
                tamedCountText.color = Color.white;
        }
    }
}