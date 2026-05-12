using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NightmareUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject nightmarePanel;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Volume Effect")]
    [SerializeField] private GameObject globalVolume;

    private bool isActive = false;

    void Start()
    {
        if (nightmarePanel != null)
            nightmarePanel.SetActive(false);

        if (globalVolume != null)
            globalVolume.SetActive(false);
    }

    public void ShowUI()
    {
        isActive = true;
        if (nightmarePanel != null)
            nightmarePanel.SetActive(true);

        if (globalVolume != null)
            globalVolume.SetActive(true);
    }

    public void HideUI()
    {
        isActive = false;
        if (nightmarePanel != null)
            nightmarePanel.SetActive(false);

        if (globalVolume != null)
            globalVolume.SetActive(false);
    }

    public void UpdateTimer(float currentTime, float maxTime)
    {
        if (!isActive) return;

        if (timerSlider != null)
            timerSlider.value = currentTime / maxTime;

        if (timerText != null)
            timerText.text = $"{Mathf.CeilToInt(currentTime)}s";
    }

    public bool IsActive() => isActive;

    public bool HasNightmareOnScene()
    {
        AnimalName[] allAnimals = FindObjectsByType<AnimalName>(FindObjectsSortMode.None);
        foreach (AnimalName animal in allAnimals)
        {
            if (animal.GetRarityName() == "Nightmare")
            {
                NightmareTimer nightmare = animal.GetComponent<NightmareTimer>();
                if (nightmare != null && nightmare.enabled)
                    return true;
            }
        }
        return false;
    }

    void Update()
    {
        if (Time.frameCount % 120 == 0)
        {
            bool hasNightmare = HasNightmareOnScene();

            if (hasNightmare && !isActive)
                ShowUI();
            else if (!hasNightmare && isActive)
                HideUI();
        }
    }
}