using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using Microsoft.Win32.SafeHandles;
using UnityEngine.UIElements;


public class ShopManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform PanelPosition;
    [SerializeField] private float AnimationDuration = 0.3f;
    [SerializeField] private Vector2 hiddenPosition = new Vector2(2000, 0);

    private Vector2 showPosition;
    private bool isOpen = false;
    private Coroutine currentAnimation;



    private void Start()
    {
        showPosition = PanelPosition.anchoredPosition;
        PanelPosition.anchoredPosition = hiddenPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            ClosePanel();
    }

    public void ToggleShop()
    {
        if(isOpen)
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

        
    private IEnumerator MovePanel(Vector2 targetPosition)
    {
        Vector2 startPosition = PanelPosition.anchoredPosition;
        float timestarted = 0f;

        while (timestarted < AnimationDuration)
        {
            timestarted += Time.deltaTime;
            float t = timestarted / AnimationDuration;
            t = Mathf.SmoothStep(0, 1, t);

            PanelPosition.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        PanelPosition.anchoredPosition = targetPosition;
        currentAnimation = null;
    }
}
