using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftInterface : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float raycastDistance = 5f;

    private Camera mainCamera;
    private bool isPlayerLooking;
    private GameObject fabricCanvas;
    private GameObject craftInterface;
    private bool isOpen;

    void Start()
    {
        mainCamera = Camera.main;

        GameObject[] objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in objects)
        {
            if (obj.CompareTag("CraftInterface"))
            {
                craftInterface = obj;
                craftInterface.SetActive(false);
                break;
            }
        }

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Fabric"))
            {
                fabricCanvas = child.gameObject;
                fabricCanvas.SetActive(false);
                break;
            }
        }
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance) && hit.collider.gameObject == gameObject)
        {
            if (!isPlayerLooking)
            {
                isPlayerLooking = true;
                if (fabricCanvas != null && !isOpen)
                    fabricCanvas.SetActive(true);
            }

            if (Input.GetKeyDown(interactKey) && !isOpen && craftInterface != null)
            {
                OpenInterface();
            }
        }
        else
        {
            if (isPlayerLooking)
            {
                isPlayerLooking = false;
                if (fabricCanvas != null && !isOpen)
                    fabricCanvas.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
        {
            CloseInterface();
        }
    }

    private void OpenInterface()
    {
        isOpen = true;
        craftInterface.SetActive(true);

        if (fabricCanvas != null)
            fabricCanvas.SetActive(false);
    }

    public void CloseInterface()
    {
        isOpen = false;
        craftInterface.SetActive(false);

        if (isPlayerLooking && fabricCanvas != null)
            fabricCanvas.SetActive(true);
    }
}