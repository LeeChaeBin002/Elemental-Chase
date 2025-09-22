using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHp = 100;
    private int currentHp;
    private bool isDead = false;

    private EnemyMove enemyMove;

    [Header("UI")]
    public Slider hpBar;

    [Header("Effects")]
    public GameObject stunEffectPrefab;   // 🔹 Inspector에서 연결할 스턴 이펙트 프리팹
    private GameObject activeStunEffect;  // 현재 실행 중인 이펙트
    [Tooltip("HP바 기준 스턴 이펙트 오프셋 (위쪽)")]
    public Vector3 stunOffset = new Vector3(0, 0.3f, 0); // Inspector에서 조절 가능
    void Start()
    {
        currentHp = maxHp;
        enemyMove = GetComponent<EnemyMove>();

        if (hpBar != null)
            hpBar.maxValue = maxHp;
        hpBar.value = currentHp;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // 이미 스턴 중이면 무시

        currentHp -= amount;
        Debug.Log($"[Enemy] 피해 {amount} → 현재 HP {currentHp}");


        if (hpBar != null)
            hpBar.value = currentHp;

        if (currentHp <= 0)
        {
            StartCoroutine(StunAndRespawn());
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

      
        // 🔹 HP바를 부드럽게 회복
        if (hpBar != null)
            StartCoroutine(FillHpBarSmooth(currentHp, 0.5f));

        // 3) 이동 재개
        if (enemyMove != null)
            enemyMove.SetStunned(false);
        // 5) 스턴 이펙트 제거
        StopStunEffect();
        isDead = false;
        Debug.Log("[Enemy] 부활 완료! 체력 회복");
    }
    private IEnumerator FillHpBarSmooth(int targetValue, float duration)
    {
        float startValue = hpBar.value;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            hpBar.value = Mathf.Lerp(startValue, targetValue, time / duration);
            yield return null;
        }
        hpBar.value = targetValue; // 마지막 보정
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

    private void StopStunEffect()
    {
        if (activeStunEffect != null)
        {
            Destroy(activeStunEffect);
            activeStunEffect = null;
        }
    }
}
