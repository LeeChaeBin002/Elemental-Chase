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
        if (rewardParent != null)
            rewardParent.SetActive(true);  // 패널 켜주기
        if (dataManager == null)
        {
            Debug.LogWarning("[RewardUI] DataManager가 연결되지 않았습니다!");
            return;
        }

        Debug.Log("[RewardUI] ShowReward 호출됨");
        gameObject.SetActive(true);

        // 🔹 UI 배경 활성화
        if (rewardParent != null)
        {
            var img = rewardParent.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                img.color = new Color(1, 1, 1, 1);
        }

        int collectedCoins = ScoreManager.instance != null ? ScoreManager.instance.coinCount : 0;
        Debug.Log($"[RewardUI] 현재 코인 개수: {collectedCoins}");

        // 🔹 StringBuilder는 여기서 선언
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("클리어 보상");
        sb.AppendLine($"획득한 코인: {collectedCoins}");
        sb.AppendLine();

        // 🔹 보상 체크
        RewardData bestReward = null;

        foreach (var r in dataManager.rewards)
        {
            Debug.Log($"[RewardUI] 보상 후보: {r.Name}, 조건타입={r.ConditionType}, Threshold={r.Threshold}, Amount={r.Amount}");

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
        else
        {
            Debug.Log("[RewardUI] 조건을 만족하는 보상이 없음!");
        }

        // 🔹 클리어 보상 타입 2 처리
        foreach (var r in dataManager.rewards)
        {
            if (r.ConditionType == 2)
            {
                sb.AppendLine($"▶ {r.Name} 달성!");
                sb.AppendLine($"   재화 +{r.Amount}");
                sb.AppendLine();
            }
        }

        // 🔹 최종 UI 적용
        rewardText.text = sb.ToString();
        Debug.Log("[RewardUI] 최종 출력 텍스트:\n" + sb.ToString());
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

