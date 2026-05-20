using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelCharacterInterface : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI incomeText;
    [SerializeField] private TextMeshProUGUI incomeIntervalText;
    [SerializeField] private TextMeshProUGUI requiredFoodText;
    [SerializeField] private TextMeshProUGUI tamingProgressText;

    [Header("Settings")]
    [SerializeField] private string animalTag = "Animal";
    [SerializeField] private KeyCode tameKey = KeyCode.E;
    [SerializeField] private float tamingDuration = 2f;

    [Header("Animal Names")]
    [SerializeField] private string[] possibleNames = { "Lion", "Tiger", "Bear", "Wolf", "Fox", "Rabbit", "Elephant", "Giraffe", "Zebra", "Deer" };

    private GameObject currentHoveredAnimal;
    private float tamingTimer = 0f;
    private bool isTaming = false;
    private TameableAnimal currentTameable;
    private AnimalLimitManager limitManager;

    void Start()
    {
        statsPanel.SetActive(false);
        limitManager = FindFirstObjectByType<AnimalLimitManager>();
    }

    void Update()
    {
        Camera currentCamera = GetActiveCamera();
        if (currentCamera == null) return;

        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag(animalTag))
            {
                if (currentHoveredAnimal != hitObject)
                {
                    currentHoveredAnimal = hitObject;
                    currentTameable = hitObject.GetComponent<TameableAnimal>();
                    UpdateAnimalUI();
                    statsPanel.SetActive(true);
                }
                HandleTaming();
            }
            else if (currentHoveredAnimal != null)
            {
                CancelTaming();
                statsPanel.SetActive(false);
                currentHoveredAnimal = null;
                currentTameable = null;
            }
        }
        else if (currentHoveredAnimal != null)
        {
            CancelTaming();
            statsPanel.SetActive(false);
            currentHoveredAnimal = null;
            currentTameable = null;
        }
    }

    private Camera GetActiveCamera()
    {
        Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in allCameras)
            if (cam.gameObject.activeInHierarchy && cam.enabled) return cam;
        return Camera.main;
    }

    private void HandleTaming()
    {
        if (currentTameable == null || currentTameable.IsTamed) return;

        if (limitManager != null && !limitManager.CanTame)
        {
            if (tamingProgressText != null)
            {
                tamingProgressText.text = RarityLocalizer.GetLocalizedTamingStatus(false, false, false, 0);
                tamingProgressText.color = Color.red;
            }
            return;
        }

        if (Input.GetKeyDown(tameKey))
        {
            int neededFood = currentTameable.GetRemainingFood();
            if (CheckFoodAvailability(neededFood))
            {
                isTaming = true;
                tamingTimer = 0f;
            }
            else
            {
                if (tamingProgressText != null)
                {
                    tamingProgressText.text = RarityLocalizer.GetLocalizedTamingStatus(false, true, false, 0);
                    tamingProgressText.color = Color.red;
                }
            }
        }

        if (isTaming && Input.GetKey(tameKey))
        {
            tamingTimer += Time.deltaTime;
            float progress = tamingTimer / tamingDuration;
            int percent = Mathf.RoundToInt(progress * 100);

            if (tamingProgressText != null)
            {
                tamingProgressText.text = RarityLocalizer.GetLocalizedTamingStatus(false, true, true, percent);
                tamingProgressText.color = Color.yellow;
            }

            if (tamingTimer >= tamingDuration) CompleteTaming();
        }

        if (Input.GetKeyUp(tameKey) && isTaming) CancelTaming();
    }

    private void CompleteTaming()
    {
        int neededFood = currentTameable.GetRemainingFood();

        if (ConsumeFood(neededFood))
        {
            currentTameable.AddFood(neededFood);
            currentTameable.CompleteTaming();
            isTaming = false;
            tamingTimer = 0f;
            UpdateAnimalUI();
        }
        else
        {
            CancelTaming();
        }
    }

    private void CancelTaming()
    {
        isTaming = false;
        tamingTimer = 0f;
        UpdateAnimalUI();
    }

    private void UpdateAnimalUI()
    {
        if (currentHoveredAnimal == null) return;

        AnimalName animalNameComponent = currentHoveredAnimal.GetComponent<AnimalName>();
        if (animalNameComponent == null)
        {
            animalNameComponent = currentHoveredAnimal.AddComponent<AnimalName>();
            animalNameComponent.GenerateRandomName(possibleNames);
        }

        MonsterSpawner monsterIncome = currentHoveredAnimal.GetComponent<MonsterSpawner>();
        string rarityName = animalNameComponent.GetRarityName();
        Color rarityColor = animalNameComponent.GetRarityColor();

        if (rarityText != null)
        {
            rarityText.text = rarityName;
            rarityText.color = rarityColor;
        }

        if (nameText != null)
        {
            nameText.text = animalNameComponent.GetLocalizedName();
            nameText.color = Color.white;
        }

        if (incomeText != null && monsterIncome != null)
        {
            incomeText.text = $"{Mathf.RoundToInt(monsterIncome.GetIncome())}";
            incomeText.color = Color.white;
        }

        if (incomeIntervalText != null && monsterIncome != null)
        {
            float interval = monsterIncome.GetIncomeInterval();
            if (interval >= 60f)
                incomeIntervalText.text = $"{Mathf.RoundToInt(interval / 60f)} {RarityLocalizer.GetLocalizedPerMin()}";
            else if (interval >= 1f)
                incomeIntervalText.text = $"{Mathf.RoundToInt(interval)} {RarityLocalizer.GetLocalizedPerSec()}";
            else
                incomeIntervalText.text = $"{Mathf.RoundToInt(1f / interval)} {RarityLocalizer.GetLocalizedPerSec()}";
            incomeIntervalText.color = Color.white;
        }

        if (requiredFoodText != null && currentTameable != null)
        {
            if (currentTameable.IsTamed)
                requiredFoodText.text = "";
            else
                requiredFoodText.text = $"{RarityLocalizer.GetLocalizedFoodNeeded()} {currentTameable.GetRemainingFood()}";
        }

        if (tamingProgressText != null && currentTameable != null)
        {
            if (currentTameable.IsTamed)
            {
                tamingProgressText.text = RarityLocalizer.GetLocalizedTamingStatus(true, false, false, 0);
                tamingProgressText.color = Color.green;
            }
            else if (limitManager != null && !limitManager.CanTame)
            {
                tamingProgressText.text = RarityLocalizer.GetLocalizedTamingStatus(false, false, false, 0);
                tamingProgressText.color = Color.red;
            }
            else if (!isTaming)
            {
                tamingProgressText.text = RarityLocalizer.GetLocalizedTamingStatus(false, true, false, 0);
                tamingProgressText.color = Color.white;
            }
        }
    }

    private bool CheckFoodAvailability(int requiredAmount)
    {
        FoodSystem foodSystem = FindFirstObjectByType<FoodSystem>();
        return foodSystem != null && foodSystem.GetCurrentFood() >= requiredAmount;
    }

    private bool ConsumeFood(int amount)
    {
        FoodSystem foodSystem = FindFirstObjectByType<FoodSystem>();
        return foodSystem != null && foodSystem.ConsumeFood(amount);
    }
}