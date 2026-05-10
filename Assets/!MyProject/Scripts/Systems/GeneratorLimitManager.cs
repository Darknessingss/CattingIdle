using System;
using TMPro;
using UnityEngine;

public class GeneratorLimitManager : MonoBehaviour
{
    [SerializeField] private int maxGenerators = 3;
    [SerializeField] private TextMeshProUGUI generatorsCountText;

    private int currentGeneratorCount;

    public int MaxGenerators => maxGenerators;
    public int CurrentGeneratorCount => currentGeneratorCount;
    public bool CanBuy => currentGeneratorCount < maxGenerators;

    public event Action OnGeneratorCountChanged;

    void Start() => UpdateUI();

    public bool TryAddGenerator()
    {
        if (currentGeneratorCount < maxGenerators)
        {
            currentGeneratorCount++;
            OnGeneratorCountChanged?.Invoke();
            UpdateUI();
            Debug.Log($"Куплено генераторов: {currentGeneratorCount}/{maxGenerators}");
            return true;
        }

        Debug.Log($"Достигнут лимит генераторов! Максимум: {maxGenerators}");
        return false;
    }

    public void RemoveGenerator()
    {
        if (currentGeneratorCount > 0)
        {
            currentGeneratorCount--;
            OnGeneratorCountChanged?.Invoke();
            UpdateUI();
            Debug.Log($"Генераторов: {currentGeneratorCount}/{maxGenerators}");
        }
    }

    private void UpdateUI()
    {
        if (generatorsCountText == null) return;

        generatorsCountText.text = $"{currentGeneratorCount}/{maxGenerators}";
    }
}