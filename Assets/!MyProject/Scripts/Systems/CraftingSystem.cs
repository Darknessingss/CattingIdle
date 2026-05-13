using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    [System.Serializable]
    public class CraftRecipe
    {
        public string recipeName;
        public int resultItemId;
        public int craftTimeSeconds;
        public List<int> requiredIngredients;
    }

    [System.Serializable]
    public class Item
    {
        public int id;
        public string name;
        public Sprite icon;
        public int count;
    }

    [Header("Items")]
    [SerializeField] private Item[] allItems;

    [Header("Craft Recipes")]
    [SerializeField] private CraftRecipe[] recipes;

    [Header("UI References")]
    [SerializeField] private Transform craftButtonsContainer;
    [SerializeField] private GameObject craftButtonPrefab;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private Image[] ingredientIcons;
    [SerializeField] private TextMeshProUGUI[] ingredientCounts;
    [SerializeField] private Button craftButton;
    [SerializeField] private TextMeshProUGUI craftTimerText;

    [Header("Item Counters")]
    [SerializeField] private TextMeshProUGUI[] itemCountTexts;

    private Wallet wallet;
    private CraftRecipe selectedRecipe;
    private bool isCrafting = false;
    private float currentCraftTime;

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();

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
            {
                CompleteCrafting();
            }
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

            CraftRecipe capturedRecipe = recipe;
            button.onClick.AddListener(() => SelectRecipe(capturedRecipe));
        }
    }

    private void SelectRecipe(CraftRecipe recipe)
    {
        if (isCrafting) return;

        selectedRecipe = recipe;
        if (craftingPanel != null)
            craftingPanel.SetActive(true);

        for (int i = 0; i < ingredientIcons.Length; i++)
        {
            if (i < recipe.requiredIngredients.Count)
            {
                int ingredientId = recipe.requiredIngredients[i];
                Item ingredient = GetItemById(ingredientId);

                if (ingredient != null)
                {
                    ingredientIcons[i].gameObject.SetActive(true);
                    ingredientIcons[i].sprite = ingredient.icon;
                    ingredientCounts[i].text = $"{GetItemCount(ingredientId)}/1";
                    ingredientCounts[i].color = GetItemCount(ingredientId) >= 1 ? Color.green : Color.red;
                }
            }
            else
            {
                ingredientIcons[i].gameObject.SetActive(false);
            }
        }

        bool hasIngredients = CheckIngredients(recipe);
        if (craftButton != null)
            craftButton.interactable = hasIngredients && !isCrafting;

        if (craftTimerText != null)
            craftTimerText.text = "CRAFT";
    }

    private int GetItemCount(int id)
    {
        Item item = GetItemById(id);
        return item != null ? item.count : 0;
    }

    private bool CheckIngredients(CraftRecipe recipe)
    {
        foreach (int ingredientId in recipe.requiredIngredients)
        {
            Item item = GetItemById(ingredientId);
            if (item == null || item.count < 1)
                return false;
        }
        return true;
    }

    private void StartCrafting()
    {
        if (selectedRecipe == null || isCrafting) return;

        if (!CheckIngredients(selectedRecipe))
        {
            Debug.Log("Not enough ingredients!");
            return;
        }

        foreach (int ingredientId in selectedRecipe.requiredIngredients)
        {
            Item item = GetItemById(ingredientId);
            if (item != null)
                item.count--;
        }

        UpdateAllCounters();

        isCrafting = true;
        currentCraftTime = selectedRecipe.craftTimeSeconds;

        if (craftButton != null)
            craftButton.interactable = false;

        UpdateCraftTimerUI();
    }

    private void UpdateCraftTimerUI()
    {
        if (craftTimerText != null && isCrafting)
        {
            int minutes = Mathf.FloorToInt(currentCraftTime / 60);
            int seconds = Mathf.FloorToInt(currentCraftTime % 60);
            craftTimerText.text = $"{minutes}:{seconds:00}";
        }
        else if (craftTimerText != null && !isCrafting)
        {
            craftTimerText.text = "CRAFT";
        }
    }

    private void CompleteCrafting()
    {
        isCrafting = false;

        Item resultItem = GetItemById(selectedRecipe.resultItemId);
        if (resultItem != null)
            resultItem.count += 1;

        UpdateAllCounters();

        if (craftButton != null)
            craftButton.interactable = true;

        if (craftTimerText != null)
            craftTimerText.text = "CRAFT";

        Debug.Log($"Crafted {selectedRecipe.recipeName}!");

        if (craftingPanel != null)
            craftingPanel.SetActive(false);
        selectedRecipe = null;
    }

    private Item GetItemById(int id)
    {
        foreach (var item in allItems)
        {
            if (item.id == id)
                return item;
        }
        return null;
    }

    private void UpdateAllCounters()
    {
        for (int i = 0; i < itemCountTexts.Length && i < allItems.Length; i++)
        {
            if (itemCountTexts[i] != null)
                itemCountTexts[i].text = allItems[i].count.ToString();
        }
    }

    public void AddItem(int id, int amount)
    {
        Item item = GetItemById(id);
        if (item != null)
            item.count += amount;
        UpdateAllCounters();
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