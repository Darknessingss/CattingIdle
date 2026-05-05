using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyLimitButton : MonoBehaviour
{
    [SerializeField] private int limitIncrease = 1;
    [SerializeField] private int price = 100;
    [SerializeField] private int maxTotalLimit = 20;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI limitText;

    private AnimalLimitManager limitManager;
    private Wallet wallet;

    void Start()
    {
        limitManager = FindFirstObjectByType<AnimalLimitManager>();
        wallet = FindFirstObjectByType<Wallet>();

        if (buyButton != null)
            buyButton.onClick.AddListener(BuyLimit);

        if (priceText != null)
            priceText.text = $"Цена: {price}";

        UpdateLimitText();
    }

    private void BuyLimit()
    {
        if (limitManager == null || wallet == null) return;

        if (limitManager.MaxTamedAnimals >= maxTotalLimit)
        {
            Debug.Log($"Достигнут максимальный лимит! Максимум: {maxTotalLimit}");
            return;
        }

        if (wallet.SpendMoney(price))
        {
            limitManager.IncreaseMaxLimit(limitIncrease);
            UpdateLimitText();
            Debug.Log($"Лимит увеличен на {limitIncrease}! Теперь максимум: {limitManager.MaxTamedAnimals}");
        }
        else
        {
            Debug.Log("Недостаточно монет!");
        }
    }

    private void UpdateLimitText()
    {
        if (limitText != null && limitManager != null)
            limitText.text = $"+{limitIncrease}";
    }
}