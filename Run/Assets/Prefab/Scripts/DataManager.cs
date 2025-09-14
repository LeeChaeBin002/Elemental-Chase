using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TextAsset rewardCsv;
    public TextAsset scoreCsv;

    public List<RewardData> rewards;
    public List<ScoreData> scores;

    void Awake()
    {
        rewards = CsvRewardLoader.LoadRewards(rewardCsv);
        scores = CsvRewardLoader.LoadScores(scoreCsv);

        Debug.Log($"보상 {rewards.Count}개, 스코어 {scores.Count}개 불러옴");
    }
}
