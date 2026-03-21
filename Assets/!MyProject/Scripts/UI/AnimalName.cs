using UnityEngine;

public class AnimalName : MonoBehaviour
{
    public string animalName;
    public string rarityName;
    public Color rarityColor;
    public float multiplier;

    public void GenerateRandomName(string[] possibleNames)
    {
        animalName = possibleNames[Random.Range(0, possibleNames.Length)];
    }

    public void SetRarity(RarityData rarity)
    {
        rarityName = rarity.rarityName;
        rarityColor = rarity.rarityColor;
        multiplier = rarity.multiplier;

        ApplyRarityColor();
    }

    private void ApplyRarityColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = rarityColor;
        }
    }

    public string GetRarityName()
    {
        return rarityName;
    }

    public Color GetRarityColor()
    {
        return rarityColor;
    }

    public float GetMultiplier()
    {
        return multiplier;
    }
}