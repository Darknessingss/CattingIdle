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
    [SerializeField] private Transform craftButtonsContainer;
    [SerializeField] private GameObject craftButtonPrefab;
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

        CreateCraftButtons();
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

    private void CreateCraftButtons()
    {
        foreach (Transform child in craftButtonsContainer)
            Destroy(child.gameObject);

        foreach (var recipe in recipes)
        {
            GameObject buttonObj = Instantiate(craftButtonPrefab, craftButtonsContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text = recipe.recipeName;

            button.onClick.AddListener(() => SelectRecipe(recipe));
        }
    }

    private void SelectRecipe(CraftRecipeSO recipe)
    {
        if (isCrafting) return;

        selectedRecipe = recipe;
        if (craftingPanel != null)
            craftingPanel.SetActive(true);

        if (resultIconImage != null && recipe.resultItem != null)
            resultIconImage.sprite = recipe.resultItem.icon;

        foreach (var slot in ingredientSlots)
            Destroy(slot);
        ingredientSlots.Clear();

        for (int i = 0; i < recipe.ingredients.Count && i < ingredientSlotPrefabs.Length; i++)
        {
            GameObject slot = Instantiate(ingredientSlotPrefabs[i], ingredientsContainer);

            Image icon = slot.transform.Find("Icon")?.GetComponent<Image>();
            TextMeshProUGUI countText = slot.transform.Find("Count")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI nameText = slot.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();

            var ingredient = recipe.ingredients[i];

            if (icon != null)
                icon.sprite = ingredient.item.icon;

            if (nameText != null)
                nameText.text = ingredient.item.itemName;

            if (countText != null)
            {
                int currentCount = GetItemCount(ingredient.item);
                countText.text = $"{currentCount}/{ingredient.amount}";
                countText.color = currentCount >= ingredient.amount ? Color.green : Color.red;
            }

            ingredientSlots.Add(slot);
        }

        bool hasIngredients = CheckIngredients(recipe);
        if (craftButton != null)
            craftButton.interactable = hasIngredients && !isCrafting;

        if (craftTimerText != null)
            craftTimerText.text = "CRAFT";
    }

    private int GetItemCount(ItemSO item)
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
            TextMeshProUGUI countText = ingredientSlots[i].transform.Find("Count")?.GetComponent<TextMeshProUGUI>();

            if (countText != null)
            {
                int currentCount = GetItemCount(ingredient.item);
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
                itemCounts.TryGetValue(allItems[i], out int count);
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
        if (craftTimerText != null)
            craftTimerText.text = "CRAFT";
        isCrafting = false;
    }
}