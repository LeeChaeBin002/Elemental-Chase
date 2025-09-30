using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;       // 점수
    public int coinCount = 0;   // 코인 개수 카운트


    public TextMeshProUGUI coinText;
    public TextMeshProUGUI scoreText; // UI 텍스트 연결 (예: "Score : 0")

    private float timeCounter = 0f;
    public int distanceScoreRate = 1;//거리 초당 점수
    public int coinScoreValue = 5;    // 코인 하나당 점수
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    void Update()
    {
        // 매 프레임마다 시간 누적
        timeCounter += Time.deltaTime;

        if (timeCounter >= 1f) // 1초마다 점수 증가
        {
            score += distanceScoreRate;
            UpdateUI();
            timeCounter -= 1f; // 잔여 시간 보존
        }
    }
    // 🔹 코인 획득 처리
    public void AddCoin()
    {
        coinCount++;               // 코인 개수 +1
        score += coinScoreValue;   // 점수 +5
        UpdateUI();
    }
  
    private void UpdateUI()
    {
     
        if (scoreText != null)
        {
            scoreText.text = "" + score;//Score : 
        }
        if (coinText != null)
        {
            coinText.text = " "+ coinCount;//coin :
        }
    }
}
