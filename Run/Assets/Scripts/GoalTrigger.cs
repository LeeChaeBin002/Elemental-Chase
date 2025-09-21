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

            // 🔹 GameManager에 RewardUI를 맡기고 직접 호출
            GameManager.Instance.ShowRewardUI();
        }
    }


}
