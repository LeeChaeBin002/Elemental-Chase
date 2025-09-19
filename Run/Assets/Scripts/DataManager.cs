using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    public List<RewardData> rewards;
    public List<ScoreData> scores;
    public static DataManager Instance { get; private set; }

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // 전역 유지하고 싶으면 추가
        }
        else
        {
            Destroy(gameObject); // 중복 생기면 제거
            return;
        }
        // ✅ CSV 파일 로드 (StreamingAssets 기준)
        string rewardPath = Path.Combine(Application.streamingAssetsPath, "CSV/Reward.csv");
        string scorePath = Path.Combine(Application.streamingAssetsPath, "CSV/Score.csv");
        
        rewards = CsvRewardLoader.LoadRewards(LoadCsvText(rewardPath));
        scores = CsvRewardLoader.LoadScores(LoadCsvText(scorePath));

        Debug.Log($"보상 {rewards.Count}개, 스코어 {scores.Count}개 불러옴");
    }
    /// <summary>
    /// StreamingAssets에서 CSV 내용을 string으로 읽음
    /// </summary>
    private string LoadCsvText(string path)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // ✅ Android 빌드는 UnityWebRequest 필요
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(path))
        {
            www.SendWebRequest();
            while (!www.isDone) { }
            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                return www.downloadHandler.text;
            else
            {
                Debug.LogError($"CSV 로드 실패: {path}, 에러: {www.error}");
                return "";
            }
        }
#else
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: {path}");
            return "";
        }
#endif
    }
}

