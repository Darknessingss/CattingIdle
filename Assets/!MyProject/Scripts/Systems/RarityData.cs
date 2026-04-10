using UnityEngine;

[System.Serializable]
public class RarityData
{
    public string rarityName = "Common";
    public float minMultiplier = 1f;
    public float maxMultiplier = 1f;
    [Range(0f, 100f)] public float spawnChance = 50f;
    public Color rarityColor = Color.white;
    public float requiredFood = 10f;

    public float multiplier
    {
        get
        {
            if (minMultiplier == maxMultiplier)
                return minMultiplier;
            return Random.Range(minMultiplier, maxMultiplier);
        }
    }
}