using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelCharacterInterface : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TextMeshProUGUI nameText;

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
                    nameText.text = animalNameComponent.animalName;
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

public class AnimalName : MonoBehaviour
{
    public string animalName;

    public void GenerateRandomName(string[] possibleNames)
    {
        animalName = possibleNames[Random.Range(0, possibleNames.Length)];
    }
}