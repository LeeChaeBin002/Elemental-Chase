using UnityEngine;

public class Effect : MonoBehaviour
{
    public MapObject effectData;
    private float originalSpeed;

    //[Header("이펙트 프리팹")]
    //public GameObject explosionEffectPrefab;

    //[Header("Fire Point")]
    //public Transform firePoint;   // 투사체가 발사될 위치

    //[Header("Prefabs")]
    //public GameObject projectilePrefab;

    void Awake()
    {
    
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (effectData == null)
            {
                Debug.LogWarning($"{gameObject.name} 의 effectData가 설정되지 않았습니다!");
                return;
            }

            PlayerMovement pm = other.GetComponent<PlayerMovement>();
                if (pm == null) return;

                
                // 예: 버프 ID별로 감소율 다르게
                switch (effectData.buffId)
                {
                    case 324020:
                        originalSpeed = pm.runSpeed;
                        pm.runSpeed *= 0.2f;//진흙구덩이 20% 이속 감소->80% 변경(테스트)
                        break;
                    case 324040:
                        originalSpeed = pm.runSpeed;
                        pm.runSpeed *= 0.6f;//바위장애물 3초간 40% 이속 감소
                        break;
                    case 321060: // 물폭탄 : 닿으면 몬스터 이속 60% 감소(3초)

                    EnemyMove[] enemies = FindObjectsOfType<EnemyMove>();
                    foreach (var enemy in enemies)
                    {
                        if (enemy != null)
                        {
                            Debug.Log($"[물폭탄 발동] {enemy.name} 3초간 이속 60% 감소");
                            enemy.ApplySlow(0.4f, 3f);
                        }
                    }

                    // 🔹 여기서 물폭탄 이펙트 한번만 생성 가능
                    //Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

                    Destroy(gameObject); // 아이템 삭제
                    break;

                case 311060: // 바람통로 : 닿는 동안 이속 60% 증가
                        originalSpeed = pm.runSpeed;
                        pm.runSpeed *= 1.6f;
                        break;
               

                }

            switch(effectData.stateId)
            {
                case 31110:
                    pm.ApplyStun(2f);//넝쿨: 2초간 스턴
                    break;
                case 32001:
                    pm.ApplyBlind(3f); //머드칠: 3초간 시야 차단
                    break;
            }
                    Debug.Log($"[효과 발동]\n" +
                    $"- Name: {effectData.name}\n" +
                    $"- ID: {effectData.id}\n" +
                    $"- BuffId: {effectData.buffId}\n" +
                    $"- StateId: {effectData.stateId}\n" +
                    $"- Desc: {effectData.description}");

                //Debug.Log($"{effectData.name} 발동 → {effectData.description}");
            }

        
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (effectData == null) return;
        if (effectData.buffId == 321060)
            return;
        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null) return;

        // 효과 끝났을 때 속도 원래대로 복구
        pm.runSpeed = originalSpeed;

        Debug.Log($"{effectData.name} 효과 종료 → 속도 복구");
    }
    void ShootAtNearestEnemy(PlayerMovement player)
    {
        EnemyMove[] enemies = FindObjectsOfType<EnemyMove>();
        EnemyMove nearest = null;
        float minDist = Mathf.Infinity;

        foreach (EnemyMove enemy in enemies)
        {
            float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        //if (nearest != null && projectilePrefab != null)
        //{
        //    Vector3 spawnPos = firePoint != null ? firePoint.position : player.transform.position;
        //    GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        //    Projectile p = proj.GetComponent<Projectile>();
        //    if (p != null)
        //    {
        //        p.SetTarget(nearest.transform); // 🔹 타겟 Transform 전달
        //    }
        //}
    }
}


