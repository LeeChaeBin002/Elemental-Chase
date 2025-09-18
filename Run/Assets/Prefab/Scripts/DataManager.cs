using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TextAsset rewardCsv;
    public TextAsset scoreCsv;

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
        rewards = CsvRewardLoader.LoadRewards(rewardCsv);
        scores = CsvRewardLoader.LoadScores(scoreCsv);

        Debug.Log($"보상 {rewards.Count}개, 스코어 {scores.Count}개 불러옴");
    }
}
