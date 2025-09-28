using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance == null) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("[GoalTrigger] Player reached the goal!");

            var pm = other.GetComponent<PlayerMovement>();
            if (pm != null) pm.enabled = false; // 골인 순간 멈추기
            // BGM 끄기
            if (StageBGMManager.Instance != null)
            {
                StageBGMManager.Instance.StopBGMWithFade();
            }
            // 🔹 GameManager에 RewardUI를 맡기고 직접 호출
            GameManager.Instance.ShowRewardUI();
        }
        // 적이 먼저 골인했을 때
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log("[GoalTrigger] Enemy reached the goal! Game Over!");

            // 플레이어 멈추기 (선택)
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var pm = player.GetComponent<PlayerMovement>();
                if (pm != null) pm.enabled = false;
            }
            // 🔹 골인 시 BGM 중지
            if (StageBGMManager.Instance != null)
            {
                StageBGMManager.Instance.StopBGMWithFade(); // 페이드 아웃으로 끄기
            }

            // 🔹 적 처리 (비활성화 or Destroy)
            gameObject.SetActive(false);
            // 게임오버 UI 호출
            GameManager.Instance.ShowLoseUI();
        }

    }
}
