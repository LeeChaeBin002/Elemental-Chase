using UnityEngine;

public class GateSpawner : MonoBehaviour
{

    [Header("이전 적")]
    public GameObject oldEnemy;


    [Header("새로 활성화할 적")]
    public GameObject newEnemy;

    private bool hasSpawned = false; // 중복 스폰 방지
    private void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;
        if (other.CompareTag("Player"))
        {
            // 1️⃣ 이전 적 비활성화
            if (oldEnemy != null && oldEnemy.activeSelf)
            {
                oldEnemy.SetActive(false);
            }

            // 2️⃣ 다음 적 활성화
            if (newEnemy != null && !newEnemy.activeSelf)
            {
                newEnemy.SetActive(true);
                Debug.Log($"{gameObject.name} 게이트 통과 → {newEnemy.name} 활성화!");
            }

            hasSpawned = true; // 중복 방지
        }
    }
}
