using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 3f;
    private bool isMoving = true;

    private float originalSpeed;

    private Renderer rend;
    private Material originalMaterial;


    [Header("상태이상 재질")]
    public Material slowMaterial;  


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
        StopAllCoroutines(); // 이전 슬로우 효과가 있으면 초기화
        StartCoroutine(SlowCoroutine(slowMultiplier, duration));
    }

    private IEnumerator SlowCoroutine(float slowMultiplier, float duration)
    {
        
        moveSpeed = originalSpeed * slowMultiplier; // 이속감소

        if (rend != null)
            rend.material = slowMaterial;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed; // 원래 속도로 복귀
        if (rend != null)
            rend.material = originalMaterial;
    }
}
