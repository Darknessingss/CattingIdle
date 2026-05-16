using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private float volume = 1f;

    private static AudioSource globalAudioSource;

    void Start()
    {
        if (globalAudioSource == null)
        {
            GameObject audioObj = GameObject.Find("GlobalButtonAudio");
            if (audioObj == null)
            {
                audioObj = new GameObject("GlobalButtonAudio");
                DontDestroyOnLoad(audioObj);
            }
            globalAudioSource = audioObj.GetComponent<AudioSource>();
            if (globalAudioSource == null)
                globalAudioSource = audioObj.AddComponent<AudioSource>();
            globalAudioSource.playOnAwake = false;
            globalAudioSource.volume = volume;
        }

        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        if (clickSound != null && globalAudioSource != null)
        {
            globalAudioSource.PlayOneShot(clickSound);
        }
    }
}