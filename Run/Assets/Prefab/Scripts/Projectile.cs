using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;              // 이동 속도
    public float rotateSpeed = 5f;         // 회전 속도 (부드럽게 따라가게)

    public GameObject hitEffectPrefab;     // 적 충돌 시 터지는 이펙트
    private Transform target;              // 추적할 목표 (적)

    public void SetTarget(Transform enemy)
    {
        target = enemy;
        Debug.Log($"[Projectile] Target set to: {enemy.name}");
    }
   
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // 타겟 없으면 사라짐
            return;
        }

        // 목표 방향 계산
        Vector3 dir = (target.position - transform.position).normalized;

        // 현재 forward에서 목표 방향으로 부드럽게 회전
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);

        // 앞으로 이동
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (hitEffectPrefab != null)
                // 충돌한 적 위치에 이펙트 생성
                Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);

            // 적에게 슬로우 효과 적용
            EnemyMove enemy = other.GetComponent<EnemyMove>();
            if (enemy != null)
            {
                Debug.Log($"[투사체 발사] {enemy.name} 느려짐");
                enemy.ApplySlow(0.4f, 3f); // 3초간 속도 60% 감소
            }

            Destroy(gameObject); // 발사체 삭제
        }
    }
}
