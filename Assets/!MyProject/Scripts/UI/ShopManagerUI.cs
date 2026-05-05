using UnityEngine;
using System.Collections;

public class ShopManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform panelPosition;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Vector2 hiddenPosition = new Vector2(2000, 0);

    private Vector2 showPosition;
    private bool isOpen;
    private Coroutine currentAnimation;

    private void Start()
    {
        showPosition = panelPosition.anchoredPosition;
        panelPosition.anchoredPosition = hiddenPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            ClosePanel();
    }

    public void ToggleShop()
    {
        if (isOpen)
            ClosePanel();
        else
            OpenPanel();
    }

    public void ClosePanel()
    {
        if (!isOpen) return;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        isOpen = false;
        currentAnimation = StartCoroutine(MovePanel(hiddenPosition));
    }

    private void OpenPanel()
    {
        if (isOpen) return;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        isOpen = true;
        currentAnimation = StartCoroutine(MovePanel(showPosition));
    }

    private IEnumerator MovePanel(Vector2 target)
    {
        Vector2 start = panelPosition.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            t = Mathf.SmoothStep(0, 1, t);
            panelPosition.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        panelPosition.anchoredPosition = target;
        currentAnimation = null;
    }
}