using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private GameObject[] tabContents;

    void Start()
    {
        for (int i = 0; i < tabContents.Length; i++)
        {
            tabContents[i].SetActive(i == 0);
        }
    }

    public void ShowTab(int index)
    {
        for (int i = 0; i < tabContents.Length; i++)
        {
            tabContents[i].SetActive(i == index);
        }
    }
}