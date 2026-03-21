using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI walletText;

    private float walletMoney = 0f;

    public float WalletMoney
    {
        get => walletMoney;
        private set
        {
            walletMoney = value;
            if (walletText != null)
            {
                walletText.text = $"{walletMoney:F0}";
            }
        }
    }

    void Start()
    {
        if (walletText != null)
        {
            walletText.text = $"{walletMoney:F0}";
        }
    }

    public void AddMoney(float amount)
    {
        WalletMoney += amount;
    }

    public bool SpendMoney(float amount)
    {
        if (WalletMoney >= amount)
        {
            WalletMoney -= amount;
            return true;
        }
        return false;
    }
}