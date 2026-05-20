using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Collections;

public class Localize : MonoBehaviour
{
    [SerializeField] private Button russianButton;
    [SerializeField] private Button englishButton;

    void Start()
    {
        if (russianButton != null)
            russianButton.onClick.AddListener(() => SetLanguage("ru"));

        if (englishButton != null)
            englishButton.onClick.AddListener(() => SetLanguage("en"));

        StartCoroutine(LoadSavedLanguage());
    }

    private IEnumerator LoadSavedLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;

        if (PlayerPrefs.HasKey("GameLanguage"))
        {
            string savedLang = PlayerPrefs.GetString("GameLanguage");
            SetLanguage(savedLang);
        }
    }

    public void SetLanguage(string language)
    {
        if (!LocalizationSettings.InitializationOperation.IsDone)
        {
            StartCoroutine(SetLanguageAfterInit(language));
            return;
        }

        var locale = LocalizationSettings.AvailableLocales.GetLocale(language);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            PlayerPrefs.SetString("GameLanguage", language);
            RarityLocalizer.SetLanguage(language);
            Debug.Log($"Language changed to: {language}");
        }
        else
        {
            Debug.LogWarning($"Locale {language} not found!");
        }
    }

    private IEnumerator SetLanguageAfterInit(string language)
    {
        yield return LocalizationSettings.InitializationOperation;
        SetLanguage(language);
    }
}