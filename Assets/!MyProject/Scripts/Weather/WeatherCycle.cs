using UnityEngine;

public class WeatherCycle : MonoBehaviour
{
    [SerializeField] private Light sunlight;

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
        if (sunlight == null)
            sunlight = GetComponent<Light>();

        sunlight.color = dayColor;
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
                sunlight.color = Color.Lerp(dayColor, nightColor, t);

                if (stateTimer >= transitionDuration)
                {
                    currentState = DayState.Night;
                    stateTimer = 0f;
                    sunlight.color = nightColor;
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
                sunlight.color = Color.Lerp(nightColor, dayColor, t2);

                if (stateTimer >= transitionDuration)
                {
                    currentState = DayState.Day;
                    stateTimer = 0f;
                    sunlight.color = dayColor;
                }
                break;
        }
    }
}