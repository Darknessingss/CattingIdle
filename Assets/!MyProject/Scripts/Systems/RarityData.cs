using UnityEngine;

[System.Serializable]
public class RarityData
{
    public string rarityName = "Common";
    public float multiplier = 1f;
    [Range(0f, 100f)] public float spawnChance = 50f;
    public Color rarityColor = Color.white;
}