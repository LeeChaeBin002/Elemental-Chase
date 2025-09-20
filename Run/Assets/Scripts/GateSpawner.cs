using UnityEngine;

public class GateSpawner : MonoBehaviour
{
    [Header("새로 스폰할 적 프리팹")]
    public GameObject enemyToActivate;

    //[Header("스폰 위치")]
    //public Transform spawnPoint;

    private bool hasSpawned = false; // 중복 스폰 방지
    private void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;
        if (other.CompareTag("Player"))
        {
            // 🔹 현재 씬에 있는 첫 번째 EnemyMove 찾기
            EnemyMove oldEnemy = FindObjectOfType<EnemyMove>();
            if (oldEnemy != null)
            {
                Destroy(oldEnemy.gameObject); // 기존 적 삭제
            }

            if (enemyToActivate != null)
            {
                enemyToActivate.SetActive(true);
                Debug.Log($"{gameObject.name} 게이트 통과 → 새로운 적 활성화!");
            }
            hasSpawned = true;
        }
    }
}
