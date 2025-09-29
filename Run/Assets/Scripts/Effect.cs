using UnityEngine;

public class Effect : MonoBehaviour
{
    public MapObject effectData;
    private float originalSpeed;
    [Header("Audio")]
    public AudioSource audioSource;     // 이펙트 오브젝트에 붙은 AudioSource
    public AudioClip slowSound;         // 진흙/감속 사운드
    public AudioClip waterBombSound;    // 물폭탄 사운드
    public AudioClip windBuffSound;     // 바람통로 사운드
    public AudioClip stunSound;         // 스턴 사운드
    public AudioClip fireballSound;     // 파이어볼 구슬 사운드
    public AudioClip megaFireballSound; // 메가 파이어볼 구슬 사운드
    public AudioClip rockHitSound;      // 바위 충돌 사운드
                                        // 공용 함수 추가 (Effect.cs 맨 위 클래스 안에)
    private void PlayAndDestroy(AudioClip clip)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio"); // 임시 오브젝트
        tempGO.transform.position = transform.position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.spatialBlend = 0f;    // 0 = 2D 사운드 (카메라 위치랑 상관없이 잘 들림)
        aSource.volume = 1f;          // 볼륨 100%
        aSource.playOnAwake = false;  // 자동 실행 막기 (명시적으로 Play() 호출)
        aSource.Play();


        Destroy(tempGO, clip.length); // 소리 다 끝나면 자동 삭제
    }
    void OnTriggerEnter(Collider other)
    {
        if (effectData == null) return;

        if (other.CompareTag("Player"))
        {

            PlayerMovement pm = other.GetComponent<PlayerMovement>();
                if (pm == null) return;

                
                // 예: 버프 ID별로 감소율 다르게
                switch (effectData.buffId)
                {
                    case 324020:
                        pm.ApplySlow(0.2f); //20->80% 이속 감소변경(테스트)
                    if (slowSound)
                        PlayAndDestroy(slowSound);
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
                    if (waterBombSound)
                        PlayAndDestroy(waterBombSound);

                    Destroy(gameObject); // 아이템 삭제
                    break;

                case 311060: // 바람통로 : 닿는 동안 이속 60% 증가
                        pm.ApplyBuff(1.6f);
                    if (windBuffSound)
                        PlayAndDestroy(windBuffSound);
                    break;
               

                }

            switch(effectData.stateId)
            {
                case 31110:
                    pm.ApplyStun(2f);//넝쿨: 2초간 스턴
                    if (stunSound)
                        PlayAndDestroy(stunSound);
                    break;
                case 32001:
                    pm.ApplyBlind(3f); //머드칠: 3초간 시야 차단

                    break;
            }

            // 맵 오브젝트 ID로 특수 효과 처리
            switch (effectData.id)
            {
                case 402007: // 파이어볼 구슬
                    EnemyHealth[] allEnemies = FindObjectsOfType<EnemyHealth>();
                    foreach (var enemy in allEnemies)
                    {
                        if (enemy != null)
                        {
                            int damage = Mathf.CeilToInt(enemy.maxHp * 0.10f);
                            enemy.TakeDamage(damage);
                            Debug.Log($"[파이어볼 구슬 발동] {enemy.name} → 최대 HP의 10%({damage}) 피해");
                        }

                        if (fireballSound)
                            PlayAndDestroy(fireballSound);
                    }

                    Destroy(gameObject); // 구슬은 1회용
                    break;

                case 402008: // 메가 파이어볼 구슬
                    EnemyHealth[] megaEnemies = FindObjectsOfType<EnemyHealth>();
                    foreach (var enemy in megaEnemies)
                    {
                        if (enemy != null)
                        {
                            int damage = Mathf.CeilToInt(enemy.maxHp * 0.15f); // 🔹 15% 피해
                            enemy.TakeDamage(damage);
                            Debug.Log($"[메가 파이어볼 구슬 발동] {enemy.name} → 최대 HP의 15%({damage}) 피해");
                        }
                    }
                    if (megaFireballSound)
                        PlayAndDestroy(megaFireballSound);
                    Destroy(gameObject); // 구슬은 1회용
                    break;
            }
            Debug.Log($"[효과 발동]\n" +
                    $"- Name: {effectData.name}\n" +
                    $"- ID: {effectData.id}\n" +
                    $"- BuffId: {effectData.buffId}\n" +
                    $"- StateId: {effectData.stateId}\n" +
                    $"- Desc: {effectData.description}");

              
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
            pm.ApplyRockSlow(0.6f, 3f);
            if (rockHitSound)
                PlayAndDestroy(rockHitSound);
            Debug.Log("바위 충돌 → 3초간 40% 감속");
        }
    }

}



