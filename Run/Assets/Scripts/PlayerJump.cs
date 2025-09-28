using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float normalJumpHeight = 3f;       // 일반 점프 높이
    public float normalJumpDuration = 0.6f;   // 일반 점프 시간
    public float obstacleJumpHeight = 2.7f;     // 장애물 점프 높이
    public float obstacleJumpDuration = 0.4f; // 장애물 점프 시간
    public float landingOffsetZ = 0.2f;       // 장애물 위 착지 시 앞/뒤 오프셋

    private Rigidbody rb;
    private Animator animator;

    private bool isGrounded = true;
    private bool hasJumped = false;
    private bool isJumping = false;
    [Header("Audio")]
    public AudioSource audioSource;    // 🔹 AudioSource 컴포넌트
    public AudioClip jumpSound;        // 🔹 점프 사운드 클립
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    /// <summary>
    /// 외부에서 호출하는 점프 함수
    /// </summary>
    public void Jump(bool isBlocked)
    {
        if (!isGrounded || hasJumped) return;

        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
        hasJumped = true;
        isGrounded = false;
        animator?.SetTrigger("Jump");

        Vector3 start = transform.position;
        Ray ray = new Ray(transform.position + Vector3.up * 1f, Vector3.forward);
        if (isBlocked)
        {
            // 기본 end만 설정
            Vector3 end = start + Vector3.forward;
            float dynamicHeight = obstacleJumpHeight;
            float dynamicDuration = obstacleJumpDuration;

            Debug.Log("[장애물 점프 - isBlocked]");
            StartCoroutine(BezierJump(start, end, dynamicHeight, dynamicDuration));
        }
        else if (Physics.Raycast(ray, out RaycastHit hit, 5f, LayerMask.GetMask("Obstacle")))
        {
            // hit 안전하게 사용 가능
            Vector3 end = start + Vector3.forward;
            float dynamicHeight = obstacleJumpHeight;
            float dynamicDuration = obstacleJumpDuration;

            Bounds b = hit.collider.bounds;
            float topY = b.max.y;
            float need = (topY - transform.position.y) + 0.6f;
            dynamicHeight = Mathf.Max(obstacleJumpHeight, need);

            float landingSide = (landingOffsetZ >= 0) ? b.max.z : b.min.z;
            float zOffset = (b.extents.z * Mathf.Abs(landingOffsetZ));

            float targetZ = (landingOffsetZ >= 0)
                ? landingSide + zOffset
                : landingSide - zOffset;

            end = new Vector3(transform.position.x, topY + 0.05f, targetZ);

            Debug.Log($"[장애물 점프] 높이 {dynamicHeight}, 시간 {dynamicDuration}");
            StartCoroutine(BezierJump(start, end, dynamicHeight, dynamicDuration));
        }
        else
        {
            // 일반 점프
            Vector3 end = start + Vector3.forward * 10f;
            Debug.Log($"[일반 점프] 높이 {normalJumpHeight}, 시간 {normalJumpDuration}");
            StartCoroutine(BezierJump(start, end, normalJumpHeight, normalJumpDuration));
        }
    }

    private IEnumerator BezierJump(Vector3 start, Vector3 end, float height, float duration)
    {
        isJumping = true;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        float elapsed = 0f;
        Vector3 control = (start + end) / 2f + Vector3.up * (height * 1.5f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Bezier 곡선 계산
            Vector3 pos =
                Mathf.Pow(1 - t, 2) * start +
                2 * (1 - t) * t * control +
                Mathf.Pow(t, 2) * end;

            // 이동 적용
            rb.MovePosition(pos);
            yield return null;
        }

        // 착지 보정: 아래 레이캐스트해서 안전한 위치 찾기
        if (Physics.Raycast(end + Vector3.up * 2f, Vector3.down, out RaycastHit groundHit, 5f, LayerMask.GetMask("Obstacle", "Untagged")))
        {
            Vector3 groundPos = groundHit.point + Vector3.up * 0.05f;
            rb.MovePosition(groundPos);
        }

        isGrounded = true;
        hasJumped = false;
        isJumping = false;
        rb.isKinematic = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Untagged") || collision.gameObject.CompareTag("Obstacle"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f)
                {
                    isGrounded = true;
                    hasJumped = false;
                    return;
                }
            }
        }
    }
}
