using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHp = 100;
    private int currentHp;
    private bool isDead = false;
    public bool isBoss = false; 

    private EnemyMove enemyMove;

    [Header("UI")]
    public Slider hpBar;
    public Slider uiHpBar;

    [Header("Effects")]
    public GameObject stunEffectPrefab;   // 🔹 Inspector에서 연결할 스턴 이펙트 프리팹
    private GameObject activeStunEffect;  // 현재 실행 중인 이펙트
    [Tooltip("HP바 기준 스턴 이펙트 오프셋 (위쪽)")]
    public Vector3 stunOffset = new Vector3(0, 0.3f, 0); // Inspector에서 조절 가능
   
    void Start()
    {
        currentHp = maxHp;
        enemyMove = GetComponent<EnemyMove>();
        
        // 초기화 (둘 다 있으면 같이)
        if (hpBar != null)
        {
            hpBar.maxValue = maxHp;
            hpBar.value = currentHp;
        }

        if (uiHpBar != null)
        {
            uiHpBar.maxValue = maxHp;
            uiHpBar.value = currentHp;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // 이미 스턴 중이면 무시

        currentHp -= amount;
        Debug.Log($"[Enemy] 피해 {amount} → 현재 HP {currentHp}");


        if (hpBar != null)
            hpBar.value = currentHp;
        if (uiHpBar != null)
        {
            uiHpBar.value = currentHp;
            Debug.Log($"[EnemyHealth] UI HpBar 즉시 갱신: {uiHpBar.value}/{uiHpBar.maxValue}");

        }

        if (currentHp <= 0)
        {
            if (isBoss)
            {
                // 보스 → 부활 X, 보상 UI 표시
                Debug.Log("[Boss] 처치됨! 보상 지급");
                if (GameManager.Instance != null)
                    GameManager.Instance.ShowRewardUI();

                Destroy(gameObject); // 필요하면 제거
            }
            else
            {
                // 일반 몬스터 → 스턴 후 리스폰
                StartCoroutine(StunAndRespawn());
            }
        }
    }

    private IEnumerator StunAndRespawn()
    {
        isDead = true;


        // 1) 이동 멈추기 (스턴 적용)
        if (enemyMove != null)
            enemyMove.SetStunned(true);

        // 2) 스턴 이펙트 생성
        PlayStunEffect();
        Debug.Log("[Enemy] 스턴 상태 진입 (3초)");

        yield return new WaitForSeconds(3f);

        // 2) 체력 회복
        currentHp = maxHp;


        // 🔹 체력바 즉시 100으로 세팅
        if (hpBar != null)
        {
            hpBar.maxValue = maxHp;
            StartCoroutine(FillHpBarSmooth(hpBar, currentHp, 0.5f));
        }
        if (uiHpBar != null)
        {
            uiHpBar.maxValue = maxHp;
            StartCoroutine(FillHpBarSmooth(uiHpBar, currentHp, 0.5f));
            Debug.Log($"[EnemyHealth] UI HpBar 갱신 완료: {uiHpBar.value}/{uiHpBar.maxValue}");
        }
        // 3) 이동 재개
        if (enemyMove != null)
            enemyMove.SetStunned(false);
        // 5) 스턴 이펙트 제거
        StopStunEffect();
        isDead = false;
        Debug.Log("[Enemy] 부활 완료! 체력 회복");
    }
    private IEnumerator FillHpBarSmooth(Slider bar, int targetValue, float duration)
    {
        if (bar == null) yield break;

        float startValue = bar.value;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            bar.value = Mathf.Lerp(startValue, targetValue, time / duration);
            yield return null;
        }
        bar.value = targetValue;
    }
    private void PlayStunEffect()
    {
        StopStunEffect(); // 중복 방지
        if (stunEffectPrefab == null)
        {
            Debug.LogError("[EnemyHealth] stunEffectPrefab이 Inspector에 할당되지 않았습니다!");
            return;
        }

        // 🔹 적 본체의 콜라이더 높이 기준
        float height = 1f;
        Collider col = GetComponent<Collider>();
        if (col != null)
            height = col.bounds.size.y;

        // 본체 기준 머리 위 + 오프셋
        Vector3 pos = transform.position + Vector3.up * height + stunOffset;
        Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);

        // 부모 없이 독립적으로 생성 (Enemy의 스케일 영향 안 받음)
        activeStunEffect = Instantiate(stunEffectPrefab, pos, rot);
        activeStunEffect.transform.localScale = Vector3.one * 2f; // 절대 스케일 2배

        // 3초 뒤 자동 제거
        Destroy(activeStunEffect, 3f);
    }
    void OnDisable()
    {
    }
    private void StopStunEffect()
    {
        if (activeStunEffect != null)
        {
            Destroy(activeStunEffect);
            activeStunEffect = null;
        }
    }
}
