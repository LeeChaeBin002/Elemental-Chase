using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 3f;
    private bool isMoving = true;
    private bool isStunned = false;
    private float originalSpeed;

    private Renderer rend;
    private Material originalMaterial;

    private Coroutine slowCoroutine;

    [Header("이펙트 프리팹")]
    public GameObject debuffEffectPrefab; // 🔹 적용할 디버프 이펙트
    private GameObject activeEffect;      // 실행 중인 이펙트


    void Start()
    {
        originalSpeed = moveSpeed;
        rend = GetComponentInChildren<Renderer>(); // 자식까지 탐색
        if (rend != null)
        {
            originalMaterial = rend.material;
        }
    }
    void Update()
    {
        if (isStunned)
        {
            animator.SetInteger("animation", 34); // Idle 애니메이션
            return; // 멈춤
        }

        if (isMoving)
        {
            // 적 캐릭터의 정면 방향으로 계속 이동
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            animator.SetInteger("animation", 18);
        }
        else
        {
            animator.SetInteger("animation", 34);
        }
    }
    // 외부에서 스턴 상태 켜고 끄는 함수
    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
        if (stunned)
        {
            moveSpeed = 0f; // 멈춤
            animator.SetInteger("animation", 34);
        }
        else
        {
            moveSpeed = originalSpeed; // 원래 속도로 복귀
            animator.SetInteger("animation", 18);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            isMoving = false; // 이동 멈추기
            animator.SetInteger("animation", 34);
            Debug.Log("Enemy reached the goal and stopped.");

            GameManager.Instance.ShowLoseUI();
        }
    }

    // 🔹 외부에서 불러쓸 “슬로우” 함수
    public void ApplySlow(float slowMultiplier, float duration)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowCoroutine(slowMultiplier, duration));
    }

    private IEnumerator SlowCoroutine(float slowMultiplier, float duration)
    {
        
        moveSpeed = originalSpeed * slowMultiplier; // 이속감소
        PlayEffect();

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed; // 원래 속도로 복귀
        StopEffect();
        if (rend != null)
            rend.material = originalMaterial;
    }
    private void PlayEffect()
    {
        StopEffect();
        if (debuffEffectPrefab != null)
        {
            // 🔹 2f 오프셋 제거 → 적 중심에 이펙트 붙음
            activeEffect = Instantiate(debuffEffectPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    private void StopEffect()
    {
        if (activeEffect != null)
        {
            Destroy(activeEffect);
            activeEffect = null;
        }
    }
}
