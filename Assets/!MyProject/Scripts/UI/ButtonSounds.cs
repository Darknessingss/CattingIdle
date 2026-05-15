using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button == null) return;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }
}