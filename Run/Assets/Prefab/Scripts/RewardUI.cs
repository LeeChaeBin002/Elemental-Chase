using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;


public class RewardUI : MonoBehaviour
{
    public GameObject rewardParent;      // UI 패널 (Canvas 안에 있음)
    public TextMeshProUGUI rewardText;  // 보상 텍스트 표시용

    private DataManager dataManager;

    void Start()
    {
        gameObject.SetActive(false);
        dataManager = FindObjectOfType<DataManager>();
      
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
        sb.AppendLine("🎉 클리어 보상 🎉");
        sb.AppendLine($"획득한 코인: {collectedCoins}");



        foreach (var r in dataManager.rewards)
        {

            if (r.ConditionType == 1 && collectedCoins >= r.Threshold)
            {
                sb.AppendLine($"{r.Name} x{r.Amount}");
            }

            if (r.ConditionType == 2) 
            {
                rewardText.text += $"{r.Name} x{r.Amount}\n";
            }
        }

        rewardText.text = sb.ToString();
    }
}
