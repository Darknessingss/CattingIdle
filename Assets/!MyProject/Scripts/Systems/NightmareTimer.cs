using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NightmareTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeToTame = 60f;

    private float currentTime;
    private bool isTimerRunning;
    private NightmareUIManager uiManager;
    private bool isNightmare;

    void Start()
    {
        AnimalName animalName = GetComponent<AnimalName>();
        if (animalName != null)
        {
            isNightmare = animalName.GetRarityName() == "Nightmare";
        }
        else
        {
            isNightmare = false;
        }

        if (!isNightmare)
        {
            enabled = false;
            return;
        }

        currentTime = timeToTame;
        isTimerRunning = true;
        uiManager = FindFirstObjectByType<NightmareUIManager>();

        if (uiManager != null)
            uiManager.ShowUI();
    }

    void Update()
    {
        if (!isTimerRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            GameOver();
        }

        if (uiManager != null)
            uiManager.UpdateTimer(currentTime, timeToTame);
    }

    private void GameOver()
    {
        isTimerRunning = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        enabled = false;
        if (uiManager != null)
            uiManager.HideUI();
    }

    void OnDestroy()
    {
        if (uiManager != null && uiManager.IsActive())
            uiManager.HideUI();
    }
}