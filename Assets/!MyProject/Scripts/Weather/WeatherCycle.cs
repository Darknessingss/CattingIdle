using System.Collections.Generic;
using UnityEngine;

public class WeatherCycle : MonoBehaviour
{
    [Header("Light Sources")]
    [SerializeField] private List<Light> lightSources = new List<Light>();

    [Header("Colors")]
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = Color.black;

    [Header("Durations")]
    [SerializeField] private float dayDuration = 300f;
    [SerializeField] private float nightDuration = 300f;
    [SerializeField] private float transitionDuration = 10f;

    private enum DayState { Day, Night, TransitionToNight, TransitionToDay }
    private DayState currentState = DayState.Day;
    private float stateTimer = 0f;

    void Start()
    {
        if (lightSources == null || lightSources.Count == 0)
        {
            Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            lightSources = new List<Light>(allLights);
        }

        SetAllLightsColor(dayColor);
        currentState = DayState.Day;
        stateTimer = 0f;
    }

    void Update()
    {
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case DayState.Day:
                if (stateTimer >= dayDuration)
                {
                    currentState = DayState.TransitionToNight;
                    stateTimer = 0f;
                }
                break;

            case DayState.TransitionToNight:
                float t = stateTimer / transitionDuration;
                SetAllLightsColor(Color.Lerp(dayColor, nightColor, t));

                if (stateTimer >= transitionDuration)
                {
                    currentState = DayState.Night;
                    stateTimer = 0f;
                    SetAllLightsColor(nightColor);
                }
                break;

            case DayState.Night:
                if (stateTimer >= nightDuration)
                {
                    currentState = DayState.TransitionToDay;
                    stateTimer = 0f;
                }
                break;

            case DayState.TransitionToDay:
                float t2 = stateTimer / transitionDuration;
                SetAllLightsColor(Color.Lerp(nightColor, dayColor, t2));

                if (stateTimer >= transitionDuration)
                {
                    currentState = DayState.Day;
                    stateTimer = 0f;
                    SetAllLightsColor(dayColor);
                }
                break;
        }
    }

    private void SetAllLightsColor(Color color)
    {
        foreach (Light light in lightSources)
        {
            if (light != null)
            {
                light.color = color;
            }
        }
    }

    public void AddLightSource(Light newLight)
    {
        if (newLight != null && !lightSources.Contains(newLight))
        {
            lightSources.Add(newLight);
        }
    }

    public void RemoveLightSource(Light lightToRemove)
    {
        if (lightSources.Contains(lightToRemove))
        {
            lightSources.Remove(lightToRemove);
        }
    }
}