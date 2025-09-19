using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public GameObject rewardUICanvas;
    public RewardUI rewardUI;
    private void Start()
    {
        if (rewardUICanvas != null)
        {
            rewardUI = rewardUICanvas.GetComponent<RewardUI>();
            rewardUICanvas.SetActive(true); // 시작할 때 꺼두기
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("골인!");
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null) pm.enabled = false;

           
            if (rewardUI != null)
            {
                rewardUI.gameObject.SetActive(true);
                
                rewardUI.ShowReward();
            }
            else
            {
                Debug.LogWarning("RewardUI를 찾을 수 없습니다!");
            }

            // 게임 멈춤 
            Time.timeScale = 0;
        }
    }
}
