using UnityEngine;

public class Effect : MonoBehaviour
{
    public MapObject effectData;
    private float originalSpeed;
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
                        pm.runSpeed *= 0.8f;//진흙구덩이 20% 이속 감소
                        break;
                    case 324040:
                        originalSpeed = pm.runSpeed;
                        pm.runSpeed *= 0.6f;//바위장애물 3초간 40% 이속 감소
                        break;
                    case 321060: // 물폭탄 : 닿으면 몬스터 이속 60% 감소(3초)
                    
                    EnemyMove[] enemies = FindObjectsOfType<EnemyMove>();
                    Debug.Log($"충돌 발생: {effectData.buffId}");
                    foreach (var enemy in enemies)
                    {
                        if (enemy != null)
                        {
                            enemy.ApplySlow(0.4f, 3f); // 40% 속도만 유지 (즉 60% 감소)
                        }
                    }

                    Destroy(gameObject); // 물폭탄 아이템은 사용 후 사라짐
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
}


