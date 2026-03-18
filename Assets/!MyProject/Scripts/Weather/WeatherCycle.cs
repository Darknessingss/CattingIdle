using UnityEngine;

public class WeatherCycle : MonoBehaviour
{
    [SerializeField] private Light sunlight;

    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = Color.black;

    [SerializeField] private float cycleDuration = 60f;

    private float timer = 0f;

    void Start()
    {
        if (sunlight == null)
            sunlight = GetComponent<Light>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.PingPong(timer / (cycleDuration / 2), 1f);

        sunlight.color = Color.Lerp(dayColor, nightColor, t);
    }
}