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

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = rarityColor;
    }

    public Color GetRarityColor() => rarityColor;
    public float GetMultiplier() => multiplier;

    public string GetRarityName()
    {
        return RarityLocalizer.GetLocalizedRarity(rarityName);
    }
    public string GetLocalizedName()
    {
        return RarityLocalizer.GetLocalizedAnimalName(animalName);
    }
}