using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button playButton;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(StartGame);

        Time.timeScale = 0f;
    }

    private void StartGame()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);

        Time.timeScale = 1f;
    }
}