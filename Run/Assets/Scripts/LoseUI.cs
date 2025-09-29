using UnityEngine;
using TMPro;

public class LoseUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinText; // Get_Gold에 연결

    private void OnEnable()
    {
        int collectedCoins = ScoreManager.instance != null ? ScoreManager.instance.coinCount : 0;

        if (coinText != null)
            coinText.text = collectedCoins.ToString();

        Debug.Log($"[LoseUI] 현재 코인 개수: {collectedCoins}");
    }
}
