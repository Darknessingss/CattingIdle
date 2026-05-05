using System.Collections.Generic;
using UnityEngine;

public class WeatherCycle : MonoBehaviour
{
    [SerializeField] private List<Light> lightSources = new List<Light>();
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = Color.black;
    [SerializeField] private float dayDuration = 300f;
    [SerializeField] private float nightDuration = 300f;
    [SerializeField] private float transitionDuration = 10f;

    private float timer;

    void Start()
    {
        if (lightSources.Count == 0)
        {
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            lightSources.AddRange(lights);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        float fullCycle = dayDuration + transitionDuration + nightDuration + transitionDuration;
        if (timer >= fullCycle) timer = 0f;

        Color currentColor = GetCurrentColor();

        foreach (Light light in lightSources)
        {
            if (light != null)
                light.color = currentColor;
        }
    }

    private Color GetCurrentColor()
    {
        if (timer < dayDuration)
            return dayColor;

        if (timer < dayDuration + transitionDuration)
        {
            float t = (timer - dayDuration) / transitionDuration;
            return Color.Lerp(dayColor, nightColor, t);
        }

        if (timer < dayDuration + transitionDuration + nightDuration)
            return nightColor;

        float t2 = (timer - dayDuration - transitionDuration - nightDuration) / transitionDuration;
        return Color.Lerp(nightColor, dayColor, t2);
    }
}