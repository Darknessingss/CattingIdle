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

    [Header("Settings")]
    [SerializeField] private string animalTag = "Animal";

    [Header("Animal Names")]
    [SerializeField] private string[] possibleNames = { "Лев", "Тигр", "Медведь", "Волк", "Лиса", "Заяц", "Слон", "Жираф", "Зебра", "Олень" };

    private Camera mainCamera;
    private GameObject currentHoveredAnimal;

    void Start()
    {
        mainCamera = Camera.main;
        statsPanel.SetActive(false);
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag(animalTag))
            {
                if (currentHoveredAnimal != hitObject)
                {
                    currentHoveredAnimal = hitObject;

                    AnimalName animalNameComponent = hitObject.GetComponent<AnimalName>();
                    if (animalNameComponent == null)
                    {
                        animalNameComponent = hitObject.AddComponent<AnimalName>();
                        animalNameComponent.GenerateRandomName(possibleNames);
                    }

                    MonsterSpawner monsterIncome = hitObject.GetComponent<MonsterSpawner>();

                    string rarityName = animalNameComponent.GetRarityName();
                    Color rarityColor = animalNameComponent.GetRarityColor();
                    float multiplier = animalNameComponent.GetMultiplier();

                    if (rarityText != null)
                    {
                        rarityText.text = rarityName;
                        rarityText.color = rarityColor;
                    }

                    if (nameText != null)
                    {
                        nameText.text = animalNameComponent.animalName;
                        nameText.color = rarityColor;
                    }

                    if (incomeText != null && monsterIncome != null)
                    {
                        incomeText.text = $"{monsterIncome.GetIncome():F0}";
                    }

                    if (incomeIntervalText != null && monsterIncome != null)
                    {
                        float interval = monsterIncome.GetIncomeInterval();
                        if (interval >= 60f)
                        {
                            int minutes = Mathf.RoundToInt(interval / 60f);
                            incomeIntervalText.text = $"{minutes}/min";
                        }
                        else if (interval >= 1f)
                        {
                            int seconds = Mathf.RoundToInt(interval);
                            incomeIntervalText.text = $"{seconds}/sec";
                        }
                        else
                        {
                            int perSecond = Mathf.RoundToInt(1f / interval);
                            incomeIntervalText.text = $"{perSecond}/sec";
                        }
                    }

                    statsPanel.SetActive(true);
                }
            }
            else if (currentHoveredAnimal != null)
            {
                statsPanel.SetActive(false);
                currentHoveredAnimal = null;
            }
        }
        else if (currentHoveredAnimal != null)
        {
            statsPanel.SetActive(false);
            currentHoveredAnimal = null;
        }
    }
}