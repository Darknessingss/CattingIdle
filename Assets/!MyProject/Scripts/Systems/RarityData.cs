using UnityEngine;

[System.Serializable]
public class RarityData
{
    public string rarityName = "Common";
    public float minMultiplier = 1f;
    public float maxMultiplier = 1f;
    [Range(0f, 100f)] public float spawnChance = 50f;
    public Color rarityColor = Color.white;
    public int minRequiredFood = 10;
    public int maxRequiredFood = 10;

    [HideInInspector] public int requiredFood;

    public float multiplier => minMultiplier == maxMultiplier ? minMultiplier : Random.Range(minMultiplier, maxMultiplier);
}