using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class CraftRecipeSO : ScriptableObject
{
    public string recipeName;
    public float craftTimeSeconds;
    public List<Ingredient> ingredients;
    public ItemSO resultItem;
    public int resultAmount = 1;
}

[System.Serializable]
public class Ingredient
{
    public ItemSO item;
    public int amount;
}