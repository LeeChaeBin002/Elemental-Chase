using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    public float runSpeed = 5f;
    public float jumpForce = 5f;
    private bool isStunned = false;

    public GameObject blindOverlay;

    public VirtualJoystick joystick; // 조이스틱 연결용
    public Button jumpButton;

    public float fallMultiplier = 2.5f; // 낙하 가속 배율

    //private float keyHoldTime = 0f;
    private UnityEngine.Rigidbody rb; // 네임스페이스 확실히 지정
    private bool isGrounded = true;
    private bool hasJumped = false;
    private bool wasGrounded = false; // 이전 프레임의 바닥 상태
    

    public float fallSpeed = 10f; // 낙하 속도
    private bool isFalling = false;
    public Transform respawnPoint;

    private bool isDead = false;
    void Start()
    {
        rb = GetComponent<UnityEngine.Rigidbody>();
        animator.SetInteger("animation", 34);

        jumpButton.onClick.AddListener(Jump);

        if (isDead)
        {
            RespawnInstant();
        }
    }
    void Update()
    {
        if (isStunned) return;

        if (isDead) return;

        if (!isDead && transform.position.y < -5f)
        {
            Die();
            return; // 더 이상 아래 코드 실행하지 않음
        }

        // 바닥 체크
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f);

        if (!isGrounded && !isFalling) // 땅에서 떨어짐 감지
        {
            rb.isKinematic = true;   // 물리 끄고
            isFalling = true;        // 낙하시작
        }
        else if (isGrounded && isFalling) // 다시 땅에 닿았을 때
        {
            rb.isKinematic = false;  // 물리 다시 켜기
            isFalling = false;       // 낙하 상태 해제
        }

        // 만약 낙하 중이면
        if (isFalling)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        }

       
        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }
      

        // 이전 프레임 바닥 상태 저장
        wasGrounded = isGrounded;
        // 바닥 체크
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f);
        // 바닥 체크
        //animator.SetBool("isGrounded", isGrounded);


        // 착지했을 때 점프 상태 리셋
        if (!wasGrounded && isGrounded)
        {
            hasJumped = false; // 착지하면 다시 점프 가능
         
        }
        //조이스틱 입력값
        Vector2 joyInput = joystick.Input;

        // 입력값 
        Vector3 moveInput = new Vector3(
             Input.GetAxisRaw("Horizontal") + joyInput.x,
            0,
            Input.GetAxisRaw("Vertical") + joyInput.y
        );

        if (moveInput.magnitude > 0.1f)
        {
            // Run
            animator.SetInteger("animation", 18);
            if (!rb.isKinematic)
                rb.velocity = moveInput.normalized * runSpeed;

            //  이동 방향으로 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveInput, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,       // 현재 회전
                targetRotation,           // 목표 회전
                Time.deltaTime * 10f      // 회전 속도
                );
        }
        else
        {
            // Idle
            animator.SetInteger("animation", 34);

            if (!rb.isKinematic)
                rb.linearVelocity = Vector3.zero;
        }
     
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
        if (!isGrounded && rb.velocity.y < 0) // 공중 + 하강 중일 때
        {
            // 중력 가속을 더해준다
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }



    void Jump()
    {
        if (isDead) return;
        if (isGrounded && !hasJumped)
            {
                animator.SetTrigger("Jump");   // 애니메이션 실행
               
            }
    }
  
    void Die()
    {
        if (isDead) return;

        isDead = true;
        if (!rb.isKinematic)
            rb.velocity = Vector3.zero;
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
        transform.position = pos;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = false;

        // Idle 상태로 되돌리기
        animator.ResetTrigger("Die");      // 트리거 초기화
        animator.SetInteger("animation", 34); // Idle 애니메이션 실행
        animator.Play("Idle", 0, 0f);

        isDead = false;
    }
    public void ResetButtonState()
    {
        //buttonPressed = false;
    }
}
