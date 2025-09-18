using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 재시작용
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    public GameObject rewardParent;      // UI 패널 (Canvas 안에 있음)
    public TextMeshProUGUI rewardText;  // 보상 텍스트 표시용

    [Header("Buttons")]
    public Button restartButton;
    public Button exitButton;

    private DataManager dataManager;

    [Header("Countdown UI")]
    public TextMeshProUGUI countdownText;

    void Start()
    {
        gameObject.SetActive(false);
        dataManager = DataManager.Instance;

        if (restartButton != null)
            restartButton.onClick.AddListener(OnClickRestart);
        if (exitButton != null)
            exitButton.onClick.AddListener(OnClickExit);

    }
    public void OnClickRestart()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Retry();
        }
    }
    
    public void ShowReward()
    {
        if (dataManager == null) return;
        Debug.Log("showReward 호출됌");
        gameObject.SetActive(true);

        var img = rewardParent.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.color = new Color(1, 1, 1, 1); // 완전 불투명 흰색
        }
        int collectedCoins = ScoreManager.instance.coinCount; // 코인 개수 가져오기

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("클리어 보상");
        sb.AppendLine($"획득한 코인: {collectedCoins}");
        sb.AppendLine();

        RewardData bestReward = null;

        foreach (var r in dataManager.rewards)
        {
            Debug.Log($"RewardData: Name={r.Name}, Type={r.ConditionType}, Threshold={r.Threshold}, Amount={r.Amount}");
            if (r.ConditionType == 1 && collectedCoins >= r.Threshold)
            {
                if (bestReward == null || r.Threshold > bestReward.Threshold)
                {
                    bestReward = r;
                }
            }
        }

        if (bestReward != null)
        {
            sb.AppendLine($"▶ {bestReward.Name} 달성!");
            sb.AppendLine($"   재화 +{bestReward.Amount}");
            sb.AppendLine();
        }


        foreach (var r in dataManager.rewards)
        {
            if (r.ConditionType == 2)
            {
                sb.AppendLine($"▶ {r.Name} 달성!");
                sb.AppendLine($"   재화 +{r.Amount}");
                sb.AppendLine();
            }
        }
        rewardText.text = sb.ToString();
    }
    

    // 🔹 종료 버튼
    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중지
#else
        Application.Quit(); // 빌드된 게임 종료
#endif
    }
}

