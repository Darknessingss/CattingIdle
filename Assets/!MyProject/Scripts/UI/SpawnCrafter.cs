using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnCrafter : MonoBehaviour
{
    [SerializeField] private int itemPrice = 1000;
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxPurchaseCount = 1;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI countText;

    private Wallet wallet;
    private int currentPurchaseCount;

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();
        buyButton.onClick.AddListener(BuyAndSpawn);
        UpdateUI();
    }

    private void BuyAndSpawn()
    {
        if (currentPurchaseCount >= maxPurchaseCount || !wallet.SpendMoney(itemPrice)) return;

        Instantiate(prefabToSpawn, spawnPoint.position, prefabToSpawn.transform.rotation);
        currentPurchaseCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        bool isMax = currentPurchaseCount >= maxPurchaseCount;

        if (priceText != null)
            priceText.text = isMax ? "MAX" : $"COST: {itemPrice}";

        if (countText != null)
            countText.text = $"{currentPurchaseCount}/{maxPurchaseCount}";

        if (buyButton != null)
            buyButton.interactable = !isMax;
    }
}