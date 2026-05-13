using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Crafting/Item")]
public class ItemSO : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite icon;
}