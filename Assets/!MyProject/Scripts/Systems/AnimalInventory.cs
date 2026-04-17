using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalInventory : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera[] cameras;
    [SerializeField] private int islandCameraIndex = 0;
    [SerializeField] private Transform islandSpawnPoint;

    [Header("Delete UI")]
    [SerializeField] private GameObject deletePanel;
    [SerializeField] private TextMeshProUGUI deleteMessageText;
    [SerializeField] private Button confirmDeleteButton;
    [SerializeField] private Button cancelDeleteButton;

    [Header("UI Settings")]
    [SerializeField] private Canvas uiCanvas;

    private List<TameableAnimal> tamedAnimals = new List<TameableAnimal>();
    private List<GameObject> allAnimalsOnScene = new List<GameObject>();
    private TameableAnimal selectedAnimalForDelete;
    private int currentCameraIndex = 0;
    private AnimalLimitManager limitManager;

    void Start()
    {
        limitManager = FindFirstObjectByType<AnimalLimitManager>();

        if (deletePanel != null)
            deletePanel.SetActive(false);

        if (cameras != null && cameras.Length > 0)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i] != null)
                    cameras[i].gameObject.SetActive(i == 0);
            }
        }

        if (confirmDeleteButton != null)
            confirmDeleteButton.onClick.AddListener(ConfirmDelete);

        if (cancelDeleteButton != null)
            cancelDeleteButton.onClick.AddListener(CancelDelete);

        if (uiCanvas != null && cameras != null && cameras.Length > 0)
        {
            uiCanvas.worldCamera = cameras[0];
        }
    }

    void Update()
    {
        if (cameras != null)
        {
            for (int i = 0; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
                {
                    int cameraIndex = (i == 0) ? islandCameraIndex : i - 1;
                    if (cameraIndex < cameras.Length && cameras[cameraIndex] != null)
                    {
                        SwitchToCamera(cameraIndex);
                    }
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Delete))
        {
            if (cameras != null && currentCameraIndex < cameras.Length && cameras[currentCameraIndex] != null)
            {
                Ray ray = cameras[currentCameraIndex].ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    TameableAnimal animal = hit.collider.GetComponent<TameableAnimal>();
                    if (animal != null)
                    {
                        ShowDeleteConfirmation(animal);
                    }
                }
            }
        }
    }

    public void RegisterAnimal(GameObject animal)
    {
        if (!allAnimalsOnScene.Contains(animal))
        {
            allAnimalsOnScene.Add(animal);
        }
    }

    public void UnregisterAnimal(GameObject animal)
    {
        if (allAnimalsOnScene.Contains(animal))
        {
            allAnimalsOnScene.Remove(animal);
        }
    }

    private void SwitchToCamera(int cameraIndex)
    {
        if (cameraIndex == currentCameraIndex) return;

        if (cameras[currentCameraIndex] != null)
            cameras[currentCameraIndex].gameObject.SetActive(false);

        currentCameraIndex = cameraIndex;
        if (cameras[currentCameraIndex] != null)
            cameras[currentCameraIndex].gameObject.SetActive(true);

        if (uiCanvas != null)
        {
            uiCanvas.worldCamera = cameras[currentCameraIndex];
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool TryAddAnimal(TameableAnimal animal)
    {
        if (limitManager != null && !limitManager.CanTame)
        {
            Debug.Log($"Нельзя добавить животное! Лимит: {limitManager.CurrentTamedCount}/{limitManager.MaxTamedAnimals}");
            return false;
        }

        tamedAnimals.Add(animal);
        allAnimalsOnScene.Remove(animal.gameObject);

        animal.transform.position = GetIslandPosition(tamedAnimals.Count - 1);
        animal.transform.parent = null;

        return true;
    }

    private Vector3 GetIslandPosition(int index)
    {
        if (islandSpawnPoint == null)
            return Vector3.zero;

        float radius = 3f;
        int maxTamed = limitManager != null ? limitManager.MaxTamedAnimals : 10;
        float angle = (360f / maxTamed) * index * Mathf.Deg2Rad;
        float x = islandSpawnPoint.position.x + Mathf.Cos(angle) * radius;
        float z = islandSpawnPoint.position.z + Mathf.Sin(angle) * radius;
        return new Vector3(x, islandSpawnPoint.position.y, z);
    }

    private void ShowDeleteConfirmation(TameableAnimal animal)
    {
        selectedAnimalForDelete = animal;

        if (deletePanel != null)
        {
            if (deleteMessageText != null)
            {
                AnimalName animalName = animal.GetComponent<AnimalName>();
                string name = animalName != null ? animalName.animalName : "Животное";
                deleteMessageText.text = $"Удалить {name}?";
            }
            deletePanel.SetActive(true);
        }
    }

    public void ConfirmDelete()
    {
        if (selectedAnimalForDelete != null)
        {
            RemoveAnimal(selectedAnimalForDelete);
            selectedAnimalForDelete = null;
        }

        if (deletePanel != null)
        {
            deletePanel.SetActive(false);
        }
    }

    public void CancelDelete()
    {
        selectedAnimalForDelete = null;

        if (deletePanel != null)
        {
            deletePanel.SetActive(false);
        }
    }

    public void RemoveAnimal(TameableAnimal animal)
    {
        animal.MarkAsBeingRemoved();

        if (tamedAnimals.Contains(animal))
        {
            tamedAnimals.Remove(animal);
        }

        if (allAnimalsOnScene.Contains(animal.gameObject))
        {
            allAnimalsOnScene.Remove(animal.gameObject);
        }

        if (limitManager != null && animal.IsTamed)
        {
            limitManager.RemoveTamedAnimal();
        }

        Destroy(animal.gameObject);
        RearrangeAnimals();
    }

    private void RearrangeAnimals()
    {
        for (int i = 0; i < tamedAnimals.Count; i++)
        {
            if (tamedAnimals[i] != null)
            {
                tamedAnimals[i].transform.position = GetIslandPosition(i);
            }
        }
    }

    public int GetTamedAnimalsCount()
    {
        return tamedAnimals.Count;
    }
}