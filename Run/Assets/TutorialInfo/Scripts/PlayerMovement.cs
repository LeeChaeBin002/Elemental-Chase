using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    public float runSpeed = 5f;
    public float jumpForce = 5f;

    public VirtualJoystick joystick; // 조이스틱 연결용
    public Button jumpButton;

    public float fallMultiplier = 2.5f; // 낙하 가속 배율

    private float keyHoldTime = 0f;
    private UnityEngine.Rigidbody rb; // 네임스페이스 확실히 지정
    private bool isGrounded = true;
    private bool hasJumped = false;
    private bool wasGrounded = false; // 이전 프레임의 바닥 상태
    private bool buttonPressed = false;

    public float fallSpeed = 10f; // 낙하 속도
    private bool isFalling = false;

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
        if (isDead) return;

        if (!isDead && transform.position.y < -10f)
        {
            Die();
            return; // 더 이상 아래 코드 실행하지 않음
        }

        // 바닥 체크
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

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
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        // 바닥 체크
        //animator.SetBool("isGrounded", isGrounded);


        // 착지했을 때 점프 상태 리셋
        if (!wasGrounded && isGrounded)
        {
            hasJumped = false; // 착지하면 다시 점프 가능
            buttonPressed = false;
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
            rb.linearVelocity = moveInput.normalized * runSpeed;

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
            rb.linearVelocity = Vector3.zero;
        }
     
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

            }
    }
   

    IEnumerator WaitForDieAnimation()
    {
        // Die 상태로 들어갈 때까지 대기
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsTag("Die"))  // 상태 이름 대신 Tag로 체크
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        // 애니메이션 길이만큼 대기
        yield return new WaitForSeconds(stateInfo.length);

        RespawnInstant();
    }
    void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        animator.SetTrigger("Die");

        StartCoroutine(WaitForDieAnimation());
    }
    public void OnDieAnimationEnd()
    {
        // 애니메이션 끝났을 때 호출됨
        RespawnInstant();
    }

    IEnumerator Respawn()
    {
        yield return null;
        // 리스폰 처리
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = false;
        animator.SetInteger("animation", 34); // Idle 상태로 복귀
        isDead = false;
    }

    void RespawnInstant()
    {
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = false;

        // Idle 상태로 되돌리기
        animator.ResetTrigger("Die");      // 트리거 초기화
        animator.SetInteger("animation", 34); // Idle 애니메이션 실행

        isDead = false;
    }
    public void ResetButtonState()
    {
        buttonPressed = false;
    }
}
