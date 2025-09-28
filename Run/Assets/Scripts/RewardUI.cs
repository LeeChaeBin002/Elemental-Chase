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
    public Button[] exitButtons;

    private DataManager dataManager;

    [Header("Countdown UI")]
    public TextMeshProUGUI countdownText;

    [Header("Audio")]
    public AudioSource audioSource;    // 🔹 AudioSource 컴포넌트
    public AudioClip WinSound;        // 🔹 사운드 클립
    [Header("Stars")]
    public GameObject[] starOnObjs;   // 채워진 별 오브젝트 3개
    public GameObject[] starOffObjs;  // 빈 별 오브젝트 3개

    [Header("Reward UI Texts")]
    public TextMeshProUGUI coinText;

    void Start()
    {
        gameObject.SetActive(false);
        dataManager = DataManager.Instance;

        if (restartButton != null)
            restartButton.onClick.AddListener(OnClickRestart);
        if (exitButtons != null)
        {
            for (int i = 0; i < exitButtons.Length; i++)
            {
                int index = i; // 🔹 클로저 문제 방지
                exitButtons[i].onClick.AddListener(() => OnClickExit(index));
            }
        }

    }
    public void OnClickExit(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0:
                Debug.Log("Exit 버튼 1 → 메인 메뉴로 이동");
                Time.timeScale = 1f;  // 씬 이동 전에 복원
                SceneManager.LoadScene("Title");
                break;

            case 1:
                Debug.Log("Exit 버튼 2 → 다음 스테이지로 이동");
                Time.timeScale = 1f;  // 씬 이동 전에 복원
                SceneManager.LoadScene("Title");
                break;

            case 2:
                Debug.Log("Exit 버튼 3 → 게임 종료");
                Time.timeScale = 1f;  // 씬 이동 전에 복원
                SceneManager.LoadScene("Title");
                break;
        }
    }
    public void OnClickRestart()
    {
        // UI 먼저 꺼주기
        if (rewardParent != null)
            rewardParent.SetActive(false);
        // 씬 리로드
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    private void UpdateStars(int starCount)
    {
        for (int i = 0; i < starOnObjs.Length; i++)
        {
            // i < starCount → 채워진 별 활성화
            starOnObjs[i].SetActive(i < starCount);
            // 나머지는 빈 별 보여주기
            starOffObjs[i].SetActive(i >= starCount);
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
        // 🔹 이겼을 때 사운드 재생
        if (audioSource != null && WinSound != null)
        {
            audioSource.PlayOneShot(WinSound);
           
        }
        // 🔹 UI 배경 활성화
        if (rewardParent != null)
        {
            var img = rewardParent.GetComponent<UnityEngine.UI.Image>();
         }

        int collectedCoins = ScoreManager.instance != null ? ScoreManager.instance.coinCount : 0;
        Debug.Log($"[RewardUI] 현재 코인 개수: {collectedCoins}");
        if (coinText != null)
            coinText.text = collectedCoins.ToString();

        // 🔹 별 개수 결정 (예시: 코인 개수 기준)
        int starCount = 0;
        if (collectedCoins >= 150) starCount = 3;  // 대박 보상
        else if (collectedCoins >= 100) starCount = 2;  // 중간 보상
        else if (collectedCoins >= 50) starCount = 1;  // 작은 보상
        else starCount = 0;  // 실패

        // 별 UI 업데이트
        UpdateStars(starCount);


        // 🔹 리워드 텍스트를 간결하게 변경
        string rewardMsg = "";
        switch (starCount)
        {
            case 1: rewardMsg = "작은 보상!"; break;
            case 2: rewardMsg = "중간 보상!"; break;
            case 3: rewardMsg = "대박 보상!"; break;
            default: rewardMsg = "보상 없음.."; break;
        }

        rewardText.text = rewardMsg;
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

