using System;
using TMPro;
using UnityEngine;

public class AnimalLimitManager : MonoBehaviour
{
    [SerializeField] private int maxTamedAnimals = 10;
    [SerializeField] private TextMeshProUGUI tamedCountText;

    private int currentTamedCount;

    public int MaxTamedAnimals => maxTamedAnimals;
    public int CurrentTamedCount => currentTamedCount;
    public bool CanTame => currentTamedCount < maxTamedAnimals;

    public event Action OnTamedCountChanged;

    void Start() => UpdateUI();

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

        Debug.Log($"Достигнут лимит! Максимум: {maxTamedAnimals}");
        return false;
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
        Debug.Log($"Лимит увеличен до {maxTamedAnimals}");
    }

    private void UpdateUI()
    {
        if (tamedCountText == null) return;

        tamedCountText.text = $"{currentTamedCount}/{maxTamedAnimals}";
        tamedCountText.color = currentTamedCount >= maxTamedAnimals ? Color.red : Color.white;
    }

}