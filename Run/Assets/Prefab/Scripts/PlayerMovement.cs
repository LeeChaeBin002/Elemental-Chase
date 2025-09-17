using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    public float runSpeed = 10f;
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
   // private bool wasGrounded = false; // 이전 프레임의 바닥 상태
    

    public float fallSpeed = 10f; // 낙하 속도
   // private bool isFalling = false;
    public Transform respawnPoint;

    private bool isDead = false;
    private float invincibleTimer = 0f;
    void Start()
    {
        rb = GetComponent<UnityEngine.Rigidbody>();
        animator.SetInteger("animation", 18);
        UpdateTargetPosition();
        jumpButton.onClick.AddListener(Jump);

        if (isDead)
        {
            RespawnInstant();
        }
    }
    void Update()
    {
        if (isDead || isStunned) return;

        // 앞으로 전진 (z축 고정)
        Vector3 forwardMove = Vector3.forward * runSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + forwardMove);

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
    void FixedUpdate()
    {
        if (!isGrounded && rb.linearVelocity.y < 0) // 공중 + 하강 중일 때
        {
            // 중력 가속을 더해준다
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }



    void Jump()
    {
        if (isDead) return;

        if (isGrounded && !hasJumped)
            {
                animator.SetTrigger("Jump");   // 애니메이션 실행
                                               // 기존 속도 초기화 후 위로 힘 주기
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                isGrounded = false;
                hasJumped = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // 바닥 오브젝트에 Ground 태그 지정
        {
            isGrounded = true;
            hasJumped = false;
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
        rb.MovePosition(pos);               // ✅ transform.position 대신 이거!
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
        rb.MovePosition(pos);               // ✅ 여기서도 MovePosition
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

        // 이후 충돌 처리 로직
    }
    public void ResetButtonState()
    {
        //buttonPressed = false;
    }
}
