using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TextAsset rewardCsv; // Inspector 할당 안해도 됨
    public TextAsset scoreCsv;

    public List<RewardData> rewards;
    public List<ScoreData> scores;
    public static DataManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 🔑 Inspector에서 안넣으면 Resources에서 자동 로드
        if (rewardCsv == null)
            rewardCsv = Resources.Load<TextAsset>("CSV/RewardTable");
        if (scoreCsv == null)
            scoreCsv = Resources.Load<TextAsset>("CSV/ScoreTable");
        if (rewardCsv == null || scoreCsv == null)
        {
            Debug.LogError("CSV 파일을 불러오지 못했습니다. Resources/CSV 경로 확인하세요!");
            return;
        }
        rewards = CsvRewardLoader.LoadRewards(rewardCsv);
        scores = CsvRewardLoader.LoadScores(scoreCsv);


        Debug.Log($"보상 {rewards.Count}개, 스코어 {scores.Count}개 불러옴");
    }
}
