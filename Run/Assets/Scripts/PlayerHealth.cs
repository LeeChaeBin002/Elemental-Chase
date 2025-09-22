using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 관련")]
    public int maxHp = 100;      // 최대 체력
    public int currentHp;        // 현재 체력

    [Header("리스폰 관련")]
    public Transform respawnPoint;
    public float respawnDelay = 2f;

    [Header("UI 관련")]
    public Slider hpSlider;                  // HP바 (UI Slider)
    public Transform statusIconContainer;    // 상태 아이콘이 표시될 부모 (예: Horizontal Layout Group)'

    private bool isDead = false;
    private Rigidbody rb;
    private Animator animator;

    // 버프/디버프 상태 관리용
    private List<string> activeEffects = new List<string>();
    private Dictionary<string, GameObject> effectIcons = new Dictionary<string, GameObject>();

    // 아이콘 프리팹 (예: 슬로우, 스턴 등 표시용)
    public GameObject effectIconPrefab;

    //public delegate void OnHealthChanged(int current, int max);
    //public event OnHealthChanged onHealthChanged; // UI 업데이트용 이벤트



    void Start()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        UpdateUI();
    }

    // 데미지 입기
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHp -= amount;
        currentHp = Mathf.Max(currentHp, 0);

        Debug.Log($"적 플레이어가 {amount} 피해를 입음. 현재 체력: {currentHp}/{maxHp}");

        UpdateUI();

        if (currentHp <= 0)
        {
            Die();
        }
    }

    // 회복하기
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHp += amount;
        currentHp = Mathf.Min(currentHp, maxHp);

        Debug.Log($"플레이어가 {amount} 회복. 현재 체력: {currentHp}/{maxHp}");

        UpdateUI();
    }
    // 버프/디버프 등록
    public void AddEffect(string effectName, float duration)
    {
        if (!activeEffects.Contains(effectName))
        {
            activeEffects.Add(effectName);
            Debug.Log($"[버프 시작] {effectName} (지속시간 {duration}s)");

            // UI 아이콘 추가
            if (effectIconPrefab != null && statusIconContainer != null)
            {
                GameObject icon = Instantiate(effectIconPrefab, statusIconContainer);
                icon.name = effectName;
                icon.GetComponentInChildren<Text>().text = effectName; // 텍스트 표시
                effectIcons[effectName] = icon;
            }

            StartCoroutine(RemoveEffectAfterDelay(effectName, duration));
        }

    }


    private IEnumerator RemoveEffectAfterDelay(string effectName, float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveEffect(effectName);
    }

    private void RemoveEffect(string effectName)
    {
        if (activeEffects.Contains(effectName))
        {
            activeEffects.Remove(effectName);
            Debug.Log($"[버프 종료] {effectName}");

            // UI 아이콘 제거
            if (effectIcons.ContainsKey(effectName))
            {
                Destroy(effectIcons[effectName]);
                effectIcons.Remove(effectName);
            }
        }
    }

    // 죽음 처리
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("플레이어 사망!");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 리스폰 코루틴 시작
        StartCoroutine(RespawnAfterDelay(respawnDelay));
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Respawn();
    }

    private void Respawn()
    {
        // 위치 초기화
        transform.position = respawnPoint != null ? respawnPoint.position : Vector3.zero;

        // 체력 복구
        currentHp = maxHp;
        isDead = false;

        if (rb != null) rb.isKinematic = false;
        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.SetInteger("animation", 34); // Idle
        }

        // 모든 효과 초기화
        activeEffects.Clear();
        StopAllCoroutines();

        // UI 초기화
        UpdateUI();
        ClearAllEffectIcons();

        Debug.Log("플레이어 리스폰 완료! 상태 초기화");
    }

    private void UpdateUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }
    }

    private void ClearAllEffectIcons()
    {
        if (statusIconContainer != null)
        {
            foreach (Transform child in statusIconContainer)
            {
                Destroy(child.gameObject);
            }
        }
        effectIcons.Clear();
    }
}