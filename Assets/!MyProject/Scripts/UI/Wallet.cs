using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI walletText;
    [SerializeField] private float walletMoney = 50f;

    public float WalletMoney => walletMoney;

    void Start()
    {
        UpdateUI();
    }

    public void AddMoney(float amount)
    {
        walletMoney += amount;
        UpdateUI();
    }

    public bool SpendMoney(float amount)
    {
        if (walletMoney >= amount)
        {
            walletMoney -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        if (walletText != null)
        {
            walletText.text = $"{Mathf.RoundToInt(walletMoney)}";
        }
    }
}