using UnityEngine;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{
    [SerializeField] private CraftRecipeSO recipe;
    [SerializeField] private CraftingSystem craftingSystem;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(() => craftingSystem.SelectRecipe(recipe));
    }
}