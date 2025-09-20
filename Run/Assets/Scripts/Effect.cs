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
                        pm.ApplySlow(0.2f); //20->80% 이속 감소변경(테스트)
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

                    
                    Destroy(gameObject); // 아이템 삭제
                    break;

                case 311060: // 바람통로 : 닿는 동안 이속 60% 증가
                        pm.ApplyBuff(1.6f);
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
    
        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null) return;

        switch (effectData.buffId)
        {
            case 324020: // 진흙 구덩이
                pm.RemoveSlow();
                break;

            case 311060: // 바람통로
                pm.RemoveBuff();
                break;
        }

        Debug.Log($"{effectData.name} 효과 종료 → 속도 복구");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (effectData == null) return;

        PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();
        if (pm == null) return;

        if (effectData.buffId == 324040) // 바위 (충돌형 → 3초간 느려짐)
        {
            pm.ApplyTimedSlow(0.6f, 3f);
            Debug.Log("바위 충돌 → 3초간 40% 감속");
        }
    }

}



