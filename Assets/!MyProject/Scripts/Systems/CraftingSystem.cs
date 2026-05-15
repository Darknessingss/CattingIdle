using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    [Header("Craft Recipes")]
    [SerializeField] private CraftRecipeSO[] recipes;
    [SerializeField] private GameObject[] ingredientSlotPrefabs;
    [SerializeField] private TextMeshProUGUI[] itemCountTexts;
    [SerializeField] private ItemSO[] allItems;

    [Header("UI References")]
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private Transform ingredientsContainer;
    [SerializeField] private Button craftButton;
    [SerializeField] private TextMeshProUGUI craftTimerText;
    [SerializeField] private Image resultIconImage;

    private Dictionary<ItemSO, int> itemCounts = new Dictionary<ItemSO, int>();
    private CraftRecipeSO selectedRecipe;
    private bool isCrafting = false;
    private float currentCraftTime;
    private List<GameObject> ingredientSlots = new List<GameObject>();

    void Start()
    {
        foreach (var item in allItems)
            itemCounts[item] = 0;

        if (craftButton != null)
            craftButton.onClick.AddListener(StartCrafting);

        if (craftingPanel != null)
            craftingPanel.SetActive(false);

        if (resultIconImage != null)
            resultIconImage.gameObject.SetActive(false);

        UpdateAllCounters();
    }

    private void Update()
    {
        if (isCrafting)
        {
            currentCraftTime -= Time.deltaTime;
            UpdateCraftTimerUI();
            if (currentCraftTime <= 0f)
                CompleteCrafting();
        }
    }

    public void SelectRecipe(CraftRecipeSO recipe)
    {
        if (isCrafting) return;

        selectedRecipe = recipe;
        if (craftingPanel != null)
            craftingPanel.SetActive(true);

        if (resultIconImage != null && recipe.resultItem != null)
        {
            resultIconImage.sprite = recipe.resultItem.icon;
            resultIconImage.gameObject.SetActive(true);
        }

        foreach (var slot in ingredientSlots)
            Destroy(slot);
        ingredientSlots.Clear();

        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            var ingredient = recipe.ingredients[i];
            int currentCount = GetItemCount(ingredient.item);

            GameObject slot = Instantiate(ingredientSlotPrefabs[GetItemIndex(ingredient.item)], ingredientsContainer);
            slot.transform.localScale = Vector3.one;

            TextMeshProUGUI countText = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (countText != null)
            {
                countText.text = $"{currentCount}/{ingredient.amount}";
                countText.color = currentCount >= ingredient.amount ? Color.green : Color.red;
            }

            ingredientSlots.Add(slot);
        }

        bool hasIngredients = CheckIngredients(recipe);
        if (craftButton != null)
            craftButton.interactable = hasIngredients && !isCrafting;

        if (craftTimerText != null)
        {
            int minutes = Mathf.FloorToInt(recipe.craftTimeSeconds / 60);
            int seconds = Mathf.FloorToInt(recipe.craftTimeSeconds % 60);
            craftTimerText.text = $"{minutes}:{seconds:00}";
        }
    }

    private int GetItemIndex(ItemSO item)
    {
        for (int i = 0; i < allItems.Length; i++)
        {
            if (allItems[i] == item)
                return i;
        }
        return 0;
    }

    public int GetItemCount(ItemSO item)
    {
        return itemCounts.ContainsKey(item) ? itemCounts[item] : 0;
    }

    private bool CheckIngredients(CraftRecipeSO recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            if (GetItemCount(ingredient.item) < ingredient.amount)
                return false;
        }
        return true;
    }

    private void StartCrafting()
    {
        if (selectedRecipe == null || isCrafting) return;
        if (!CheckIngredients(selectedRecipe)) return;

        foreach (var ingredient in selectedRecipe.ingredients)
            itemCounts[ingredient.item] -= ingredient.amount;

        UpdateAllCounters();
        isCrafting = true;
        currentCraftTime = selectedRecipe.craftTimeSeconds;

        if (craftButton != null)
            craftButton.interactable = false;

        UpdateCraftTimerUI();
        RefreshIngredientsUI();
    }

    private void RefreshIngredientsUI()
    {
        if (selectedRecipe == null) return;

        for (int i = 0; i < ingredientSlots.Count && i < selectedRecipe.ingredients.Count; i++)
        {
            var ingredient = selectedRecipe.ingredients[i];
            int currentCount = GetItemCount(ingredient.item);

            TextMeshProUGUI countText = ingredientSlots[i].GetComponentInChildren<TextMeshProUGUI>();
            if (countText != null)
            {
                countText.text = $"{currentCount}/{ingredient.amount}";
                countText.color = currentCount >= ingredient.amount ? Color.green : Color.red;
            }
        }
    }

    private void UpdateCraftTimerUI()
    {
        if (craftTimerText != null)
        {
            if (isCrafting)
            {
                int minutes = Mathf.FloorToInt(currentCraftTime / 60);
                int seconds = Mathf.FloorToInt(currentCraftTime % 60);
                craftTimerText.text = $"{minutes}:{seconds:00}";
            }
            else
            {
                craftTimerText.text = "CRAFT";
            }
        }
    }

    private void CompleteCrafting()
    {
        isCrafting = false;

        if (selectedRecipe.resultItem != null)
        {
            if (itemCounts.ContainsKey(selectedRecipe.resultItem))
                itemCounts[selectedRecipe.resultItem] += selectedRecipe.resultAmount;
            else
                itemCounts[selectedRecipe.resultItem] = selectedRecipe.resultAmount;
        }

        UpdateAllCounters();

        if (craftButton != null)
            craftButton.interactable = true;

        if (craftTimerText != null)
            craftTimerText.text = "CRAFT";

        RefreshIngredientsUI();
        Debug.Log($"Crafted {selectedRecipe.recipeName}!");
    }

    private void UpdateAllCounters()
    {
        for (int i = 0; i < itemCountTexts.Length && i < allItems.Length; i++)
        {
            if (itemCountTexts[i] != null)
            {
                int count = itemCounts.ContainsKey(allItems[i]) ? itemCounts[allItems[i]] : 0;
                itemCountTexts[i].text = count.ToString();
            }
        }
    }

    public void AddItem(ItemSO item, int amount)
    {
        if (itemCounts.ContainsKey(item))
            itemCounts[item] += amount;
        else
            itemCounts[item] = amount;
        UpdateAllCounters();

        if (craftingPanel != null && craftingPanel.activeSelf && selectedRecipe != null)
            RefreshIngredientsUI();
    }

    public void ClosePanel()
    {
        if (craftingPanel != null)
            craftingPanel.SetActive(false);
        selectedRecipe = null;
        if (resultIconImage != null)
            resultIconImage.gameObject.SetActive(false);
        if (craftTimerText != null)
            craftTimerText.text = "CRAFT";
        isCrafting = false;
    }
}