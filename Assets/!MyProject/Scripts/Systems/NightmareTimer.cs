using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NightmareTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeToTame = 60f;

    [Header("Audio")]
    [SerializeField] private AudioClip growlSound;
    [SerializeField] private AudioClip ambientHorrorSound;
    [SerializeField] private bool loopAmbientSound = true;

    private float currentTime;
    private bool isTimerRunning;
    private NightmareUIManager uiManager;
    private bool isNightmare;
    private AudioSource growlSource;
    private AudioSource ambientSource;

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

        growlSource = gameObject.AddComponent<AudioSource>();
        growlSource.loop = false;
        growlSource.clip = growlSound;

        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop = loopAmbientSound;
        ambientSource.clip = ambientHorrorSound;

        if (ambientHorrorSound != null)
            ambientSource.Play();

        if (growlSound != null)
            growlSource.Play();

        InvokeRepeating("PlayGrowl", 5f, 10f);

        currentTime = timeToTame;
        isTimerRunning = true;
        uiManager = FindFirstObjectByType<NightmareUIManager>();

        if (uiManager != null)
            uiManager.ShowUI();
    }

    private void PlayGrowl()
    {
        if (isTimerRunning && growlSound != null)
            growlSource.Play();
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
        CancelInvoke("PlayGrowl");

        if (growlSource != null)
            growlSource.Stop();
        if (ambientSource != null)
            ambientSource.Stop();

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        enabled = false;
        CancelInvoke("PlayGrowl");

        if (growlSource != null)
            growlSource.Stop();
        if (ambientSource != null)
            ambientSource.Stop();

        if (uiManager != null)
            uiManager.HideUI();
    }

    void OnDestroy()
    {
        CancelInvoke("PlayGrowl");

        if (growlSource != null)
            growlSource.Stop();
        if (ambientSource != null)
            ambientSource.Stop();

        if (uiManager != null && uiManager.IsActive())
            uiManager.HideUI();
    }
}