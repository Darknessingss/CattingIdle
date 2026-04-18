using System.Collections.Generic;
using UnityEngine;

public class OutlineModel : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private List<Camera> cameras = new List<Camera>();

    [Header("Outline Settings")]
    [SerializeField] private string animalTag = "Animal";
    [SerializeField] private Color highlightColor = Color.black;
    [SerializeField] private float outlineWidth = 6f;

    private GameObject currentHighlightedAnimal;
    private Outline currentOutline;

    void Update()
    {
        Camera activeCamera = GetActiveCamera();
        if (activeCamera == null) return;

        Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag(animalTag))
            {
                if (currentHighlightedAnimal != hitObject)
                {
                    ClearHighlight();
                    currentHighlightedAnimal = hitObject;

                    if (!hitObject.TryGetComponent(out currentOutline))
                    {
                        currentOutline = hitObject.AddComponent<Outline>();
                    }

                    currentOutline.OutlineMode = Outline.Mode.OutlineAll;
                    currentOutline.OutlineColor = highlightColor;
                    currentOutline.OutlineWidth = outlineWidth;
                    currentOutline.enabled = true;
                }
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    private Camera GetActiveCamera()
    {
        foreach (Camera cam in cameras)
        {
            if (cam != null && cam.isActiveAndEnabled)
                return cam;
        }
        return Camera.main;
    }

    private void ClearHighlight()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
        currentHighlightedAnimal = null;
    }

    void OnDestroy()
    {
        ClearHighlight();
    }
}