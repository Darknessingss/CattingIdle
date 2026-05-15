using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftInterface : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private GameObject fabricCanvas;
    private GameObject craftInterface;
    private bool isOpen;
    private Camera currentCamera;

    void Start()
    {
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
        currentCamera = GetActiveCamera();
        if (currentCamera == null) return;

        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject == gameObject)
        {
            if (fabricCanvas != null && !isOpen && !fabricCanvas.activeSelf)
                fabricCanvas.SetActive(true);

            if (Input.GetKeyDown(interactKey) && !isOpen && craftInterface != null)
            {
                OpenInterface();
            }
        }
        else
        {
            if (fabricCanvas != null && fabricCanvas.activeSelf && !isOpen)
                fabricCanvas.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
        {
            CloseInterface();
        }
    }

    private Camera GetActiveCamera()
    {
        Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in allCameras)
        {
            if (cam.gameObject.activeInHierarchy && cam.enabled)
                return cam;
        }
        return Camera.main;
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

        if (fabricCanvas != null)
            fabricCanvas.SetActive(true);
    }
}