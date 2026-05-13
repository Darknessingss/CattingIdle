using TMPro;
using UnityEngine;

public class GoldenFox : MonoBehaviour
{
    [Header("Golden Fox Settings")]
    [SerializeField] private float lifeTime = 30f;
    [SerializeField] private float incomeMultiplier = 10f;

    private float currentTime;
    private bool isActive = true;
    private bool isTamed = false;
    private MonsterSpawner monsterSpawner;
    private TextMeshProUGUI timerText;
    private GameObject floatingText;
    private TameableAnimal tameableAnimal;

    void Start()
    {
        monsterSpawner = GetComponent<MonsterSpawner>();
        tameableAnimal = GetComponent<TameableAnimal>();
        currentTime = lifeTime;

        CreateTimerUI();
        ApplyGoldenMultiplier();
    }

    void Update()
    {
        if (!isActive) return;

        if (tameableAnimal != null && tameableAnimal.IsTamed)
        {
            if (!isTamed)
            {
                isTamed = true;
                OnTamed();
            }
            return;
        }

        currentTime -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTime <= 0f)
        {
            Despawn();
        }
    }

    private void OnTamed()
    {
        if (floatingText != null)
            floatingText.SetActive(false);

        Debug.Log("Golden fox tamed! It will not disappear.");
    }

    private void CreateTimerUI()
    {
        floatingText = new GameObject("GoldenFoxTimer");
        floatingText.transform.SetParent(transform);
        floatingText.transform.localPosition = new Vector3(0, 1.5f, 0);

        timerText = floatingText.AddComponent<TextMeshProUGUI>();
        timerText.fontSize = 3;
        timerText.alignment = TextAlignmentOptions.Center;
        timerText.color = Color.yellow;
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = $"{Mathf.CeilToInt(currentTime)}s";
    }

    private void ApplyGoldenMultiplier()
    {
        if (monsterSpawner != null)
            monsterSpawner.ApplyIncomeMultiplier(incomeMultiplier);
    }

    private void Despawn()
    {
        isActive = false;
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (floatingText != null)
            Destroy(floatingText);
    }
}