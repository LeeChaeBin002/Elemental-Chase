using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    public float runSpeed = 10f;
    private float baseSpeed;
    private int slowCount = 0;
    public float laneOffset = 3f;
    public float jumpForce = 5f;
    public float laneChangeSpeed = 10f;//레인 이동속도
    
    private bool isStunned = false;

    public GameObject blindOverlay;

    public VirtualJoystick joystick; // 조이스틱 연결용
    public Button jumpButton;

    public float fallMultiplier = 2.5f; // 낙하 가속 배율

    //private float keyHoldTime = 0f;
    private UnityEngine.Rigidbody rb;
    private int currentLane = 1;
    private Vector3 targetPosition;

    private bool isGrounded = true;
    private bool hasJumped = false;
    private bool isBlocked = false;
    

    public float fallSpeed = 10f; // 낙하 속도
    public Transform respawnPoint;

    private bool isDead = false;
    private float invincibleTimer = 0f;
    void Start()
    {
        rb = GetComponent<UnityEngine.Rigidbody>();
        baseSpeed = runSpeed;
        rb.freezeRotation = true;
        animator.SetInteger("animation", 18);
        UpdateTargetPosition();
        jumpButton.onClick.AddListener(Jump);

        if (isDead)
        {
            RespawnInstant();
        }
    }
    public void ApplySlow(float multiplier)
    {
        //slowCount++;
        runSpeed = baseSpeed * multiplier;
        Debug.Log($"[슬로우 적용] {multiplier * 100}% 속도로 변경");
    }

    public void RemoveSlow()
    {
        runSpeed = baseSpeed;
        Debug.Log("[슬로우 종료] 기본 속도로 복구");
    }
    // 🔹 일정 시간 후 자동 해제되는 슬로우 (바위 같은 경우)
    public void ApplyTimedSlow(float multiplier, float duration)
    {
        StopCoroutine(nameof(TimedSlowCoroutine)); // 중복 방지
        StartCoroutine(TimedSlowCoroutine(multiplier, duration));
    }
    private IEnumerator TimedSlowCoroutine(float multiplier, float duration)
    {
        runSpeed = baseSpeed * multiplier;
        Debug.Log($"[슬로우 적용] {multiplier * 100}% 속도로 변경 ({duration}초)");

        yield return new WaitForSeconds(duration);

        runSpeed = baseSpeed;
        Debug.Log("[슬로우 자동 종료] 기본 속도로 복구");
    }

    public void ApplyBuff(float multiplier)
    {
        runSpeed = baseSpeed * multiplier;
        Debug.Log($"[버프 적용] {multiplier * 100}% 속도로 변경");
    }

    public void RemoveBuff()
    {
        runSpeed = baseSpeed;
        Debug.Log("[버프 종료] 기본 속도로 복구");
    }

void Update()
    {
        // 좌우 레인 변경 (키보드 입력 예시: A=왼쪽, D=오른쪽)
        if (Input.GetKeyDown(KeyCode.A))
            ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D))
            ChangeLane(1);

        // 레인 보간 이동 (x축만 움직임)
        Vector3 lanePos = new Vector3(targetPosition.x, rb.position.y, rb.position.z);
        rb.position = Vector3.MoveTowards(rb.position, lanePos, laneChangeSpeed * Time.deltaTime);

        // 애니메이션: 항상 달리는 상태
        animator.SetInteger("animation", 18);

        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }
      

    }
 

    public void ChangeLane(int direction)
    {
        int newLane = Mathf.Clamp(currentLane + direction, 0, 2); // 0~2 범위 제한
        if (newLane != currentLane)
        {
            currentLane = newLane;
            UpdateTargetPosition();
        }
    }
    void UpdateTargetPosition()
    {
        // 레인별 X 좌표 계산 (왼 -3, 중 0, 오 +3 같은 구조)
        float xPos = (currentLane - 1) * laneOffset;
        targetPosition = new Vector3(xPos, transform.position.y, transform.position.z);
    }

    public void ApplyStun(float duration)//스턴지속
    {
        StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        rb.linearVelocity = Vector3.zero;
        animator.SetInteger("animation", 34);
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }
    public void ApplyBlind(float duration)//시야가려짐
    {
        StartCoroutine(BlindCoroutine(duration));
    }
    IEnumerator BlindCoroutine(float duration)
    {
        if (blindOverlay != null) blindOverlay.SetActive(true);
        yield return new WaitForSeconds(duration);
        if (blindOverlay != null) blindOverlay.SetActive(false);
    }



    void Jump()
    {
        if (isDead) return;
        if (isDead || isStunned) return;

        if (!hasJumped && (isGrounded || isBlocked))
        {
            hasJumped = true;
            isGrounded = false;

            animator.SetTrigger("Jump");

            // 기본 점프 힘
            float force = jumpForce;

            

            if (isBlocked)
            {
                // Scale.y = 1 → 높이 1m 장애물
                float requiredJump = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * 1.3f);
                force = Mathf.Max(force, requiredJump); // 최소 4.5 이상 보정
                Debug.Log($" 장애물 앞 점프! force={force}");
            }
            else
            {
                Debug.Log(" 일반 점프!!");
            }

            // y속도 초기화 후 점프
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        }


    }
    // 점프 직후 추가 힘을 적용하는 코루틴
    private IEnumerator ApplyExtraJumpForce()
    {
        yield return new WaitForFixedUpdate(); // 한 프레임 대기

        // 추가 상승 힘 적용
        if (!isGrounded && rb.linearVelocity.y > 0)
        {
            rb.AddForce(Vector3.up * (jumpForce * 0.5f), ForceMode.Impulse);
        }
    }



    private void OnCollisionStay(Collision collision)
    {
        if (rb == null) return; // Rigidbody 없으면 무시
        foreach (ContactPoint contact in collision.contacts)
        {
            // 위에서 닿았고, 실제로 거의 내려오는 중일 때만 착지 처리
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f && rb.linearVelocity.y <= 0.1f)
            {
                isGrounded = true;
                hasJumped = false;
                return;
            }
        }
    }
    private void CheckBlocked()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f; // 캐릭터 중간 높이
        if (Physics.Raycast(origin, Vector3.forward, out hit, 1.0f))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                isBlocked = true;
                return;
            }
        }
        isBlocked = false;
    }
    void FixedUpdate()
    {
        if (isDead || isStunned) return;

        //rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, runSpeed);

        // 앞으로 전진
        Vector3 vel = rb.linearVelocity;
        vel.z = runSpeed;   // 앞으로만 강제
        vel.x = 0;          // 좌우는 레인 이동으로 제어
        rb.linearVelocity = vel; // y는 그대로 유지 (점프 값 살림)

        // 좌우 이동
        Vector3 lanePos = new Vector3(targetPosition.x, rb.position.y, rb.position.z);
        rb.MovePosition(Vector3.MoveTowards(rb.position, lanePos, laneChangeSpeed * Time.fixedDeltaTime));

        // 중력 가속 보정
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        // 앞 막힘 체크
        CheckBlocked();
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isBlocked = false;  // 막힘 해제
            Debug.Log("Obstacle 막힘 해제!");
        }
        // 또는 태그 없이 일반적인 충돌체에서 벗어날 때도 해제
        StartCoroutine(DelayedBlockCheck());
    }
    // 약간의 딜레이 후 블록 상태 재확인
    private IEnumerator DelayedBlockCheck()
    {
        yield return new WaitForSeconds(0.1f);

        // 주변에 장애물이 없으면 블록 해제
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 1.5f);
        bool hasNearbyObstacle = false;

        foreach (Collider col in nearbyColliders)
        {
            if (col != GetComponent<Collider>() && !col.isTrigger)
            {
                Vector3 direction = (transform.position - col.transform.position).normalized;
                if (Vector3.Dot(direction, Vector3.forward) < 0.3f)
                {
                    hasNearbyObstacle = true;
                    break;
                }
            }
        }

        if (!hasNearbyObstacle)
        {
            isBlocked = false;
        }
    }
    void Die()
    {
        if (isDead) return;

        isDead = true;
        if (!rb.isKinematic)
            rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        animator.SetTrigger("Die");

        StartCoroutine(RespawnAfterDelay(1.9f));
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnInstant();
        isDead = false;
    }
    public void OnDieAnimationEnd()
    {
        // 애니메이션 끝났을 때 호출됨
        RespawnInstant();
    }

  
    void RespawnInstant()
    {
        Vector3 pos = respawnPoint.position;
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 5f, Vector3.down, out hit, 20f))
        {
            pos.y = hit.point.y + 0.01f;
        }
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;              // 물리 잠깐 끄기
        rb.MovePosition(pos);               // transform.position 대신 이거!
        StartCoroutine(ReenablePhysics());  //

        // Idle 상태로 되돌리기
        animator.ResetTrigger("Die");      // 트리거 초기화
        animator.SetInteger("animation", 34); // Idle 애니메이션 실행
        animator.Play("Idle", 0, 0f);

        isDead = false;
    }
    public void RespawnAt(Transform respawnPoint)
    {
        Vector3 pos = respawnPoint.position + Vector3.up * 5f;
        RaycastHit hit;

        if (Physics.Raycast(pos, Vector3.down, out hit, 20f))
        {
            pos = hit.point + Vector3.up * 0.1f; // 바닥 위
        }

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;              // 물리 잠깐 끄기
        rb.MovePosition(pos);               // 여기서도 MovePosition
        StartCoroutine(ReenablePhysics());

        animator.ResetTrigger("Die");
        animator.SetInteger("animation", 34);
        animator.Play("Idle", 0, 0f);

        isDead = false;
        // 무적 시간 1초 시작
        invincibleTimer = 1f;
    }
    private IEnumerator ReenablePhysics()
    {
        yield return new WaitForFixedUpdate(); // 물리 프레임 한 번 기다린 뒤
        rb.isKinematic = false;                // 다시 활성화
    }
    private void OnTriggerEnter(Collider other)
    {
        if (invincibleTimer > 0f)
            return; // 무적 상태라면 충돌 무시

    }
   
}
