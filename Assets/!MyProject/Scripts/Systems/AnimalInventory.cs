using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalInventory : MonoBehaviour
{
    [SerializeField] private Camera[] cameras;
    [SerializeField] private int islandCameraIndex = 0;
    [SerializeField] private Transform islandSpawnPoint;
    [SerializeField] private GameObject deletePanel;
    [SerializeField] private TextMeshProUGUI deleteMessageText;
    [SerializeField] private Button confirmDeleteButton;
    [SerializeField] private Button cancelDeleteButton;

    private List<TameableAnimal> tamedAnimals = new List<TameableAnimal>();
    private List<GameObject> allAnimalsOnScene = new List<GameObject>();
    private TameableAnimal selectedAnimalForDelete;
    private int currentCameraIndex;
    private AnimalLimitManager limitManager;

    void Start()
    {
        limitManager = FindFirstObjectByType<AnimalLimitManager>();
        deletePanel.SetActive(false);

        for (int i = 0; i < cameras.Length; i++)
            cameras[i].gameObject.SetActive(i == 0);

        confirmDeleteButton.onClick.AddListener(ConfirmDelete);
        cancelDeleteButton.onClick.AddListener(CancelDelete);
    }

    void Update()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                int cameraIndex = i == 0 ? islandCameraIndex : i - 1;
                if (cameraIndex < cameras.Length)
                    SwitchToCamera(cameraIndex);
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Delete))
        {
            Ray ray = cameras[currentCameraIndex].ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TameableAnimal animal = hit.collider.GetComponent<TameableAnimal>();
                if (animal != null && !IsNightmareOnScene(animal.gameObject))
                    ShowDeleteConfirmation(animal);
            }
        }
    }

    private bool IsNightmareOnScene(GameObject animal)
    {
        AnimalName animalName = animal.GetComponent<AnimalName>();
        if (animalName != null && animalName.GetRarityName() == "Nightmare")
        {
            TameableAnimal tameable = animal.GetComponent<TameableAnimal>();
            if (tameable != null && !tameable.IsTamed)
            {
                Debug.Log("Cannot delete Nightmare! Tame it first!");
                return true;
            }
        }
        return false;
    }

    public void RegisterAnimal(GameObject animal)
    {
        if (!allAnimalsOnScene.Contains(animal))
            allAnimalsOnScene.Add(animal);
    }

    public void UnregisterAnimal(GameObject animal)
    {
        allAnimalsOnScene.Remove(animal);
    }

    private void SwitchToCamera(int cameraIndex)
    {
        if (cameraIndex == currentCameraIndex) return;

        cameras[currentCameraIndex].gameObject.SetActive(false);
        currentCameraIndex = cameraIndex;
        cameras[currentCameraIndex].gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool TryAddAnimal(TameableAnimal animal)
    {
        if (!limitManager.CanTame)
        {
            Debug.Log($"Лимит: {limitManager.CurrentTamedCount}/{limitManager.MaxTamedAnimals}");
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
        float radius = 3f;
        int maxTamed = limitManager.MaxTamedAnimals;
        float angle = (360f / maxTamed) * index * Mathf.Deg2Rad;
        float x = islandSpawnPoint.position.x + Mathf.Cos(angle) * radius;
        float z = islandSpawnPoint.position.z + Mathf.Sin(angle) * radius;
        return new Vector3(x, islandSpawnPoint.position.y, z);
    }

    private void ShowDeleteConfirmation(TameableAnimal animal)
    {
        selectedAnimalForDelete = animal;
        AnimalName animalName = animal.GetComponent<AnimalName>();
        deleteMessageText.text = $"Delete {animalName.animalName}?";
        deletePanel.SetActive(true);
    }

    public void ConfirmDelete()
    {
        RemoveAnimal(selectedAnimalForDelete);
        selectedAnimalForDelete = null;
        deletePanel.SetActive(false);
    }

    public void CancelDelete()
    {
        selectedAnimalForDelete = null;
        deletePanel.SetActive(false);
    }

    public void RemoveAnimal(TameableAnimal animal)
    {
        if (IsNightmareOnScene(animal.gameObject))
        {
            Debug.Log("Cannot delete Nightmare fox!");
            return;
        }

        animal.MarkAsBeingRemoved();

        if (animal.IsTamed)
        {
            limitManager.RemoveTamedAnimal();
        }

        tamedAnimals.Remove(animal);
        allAnimalsOnScene.Remove(animal.gameObject);
        Destroy(animal.gameObject);
        RearrangeAnimals();
    }

    private void RearrangeAnimals()
    {
        for (int i = 0; i < tamedAnimals.Count; i++)
            tamedAnimals[i].transform.position = GetIslandPosition(i);
    }

    public int GetTamedAnimalsCount() => tamedAnimals.Count;
}