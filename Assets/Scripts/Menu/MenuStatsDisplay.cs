using UnityEngine;
using TMPro;

public class MenuStatsDisplay : MonoBehaviour
{
    public TextMeshProUGUI totalCoinsText;
    public TextMeshProUGUI totalDiamondsText;

    void Start()
    {
        if (SaveManager.Instance != null)
        {
            totalCoinsText.text = SaveManager.Instance.TotalCoins.ToString();
            totalDiamondsText.text = SaveManager.Instance.TotalDiamonds.ToString();
        }
    }
}
