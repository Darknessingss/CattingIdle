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
    [SerializeField] private GameObject craftInterface;
    [SerializeField] private Button closeButton;

    private Wallet wallet;
    private int currentPurchaseCount;
    private GameObject spawnedCrafter;

    void Start()
    {
        wallet = FindFirstObjectByType<Wallet>();
        buyButton.onClick.AddListener(BuyAndSpawn);
        UpdateUI();

        if (craftInterface != null)
            craftInterface.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCraftInterface);
    }

    private void BuyAndSpawn()
    {
        if (currentPurchaseCount >= maxPurchaseCount || !wallet.SpendMoney(itemPrice)) return;

        spawnedCrafter = Instantiate(prefabToSpawn, spawnPoint.position, prefabToSpawn.transform.rotation);
        currentPurchaseCount++;

        if (craftInterface != null)
            craftInterface.SetActive(true);

        UpdateUI();
    }

    private void CloseCraftInterface()
    {
        if (craftInterface != null)
            craftInterface.SetActive(false);

        if (spawnedCrafter != null)
        {
            CraftInterface ci = spawnedCrafter.GetComponent<CraftInterface>();
            if (ci != null)
                ci.CloseInterface();
        }
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