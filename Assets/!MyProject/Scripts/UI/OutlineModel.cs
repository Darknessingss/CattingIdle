using UnityEngine;

public class OutlineModel : MonoBehaviour
{
    [SerializeField] private string animalTag = "Animal";
    [SerializeField] private Color highlightColor = Color.black;
    [SerializeField] private float outlineWidth = 6f;

    private Camera mainCamera;
    private GameObject currentHighlightedAnimal;
    private Outline currentOutline;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

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