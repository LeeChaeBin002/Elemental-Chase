using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    public float runSpeed = 5f;
    
    public VirtualJoystick joystick; // 조이스틱 연결용


    private float keyHoldTime = 0f;
    private UnityEngine.Rigidbody rb; // 네임스페이스 확실히 지정

    void Start()
    {
        rb = GetComponent<UnityEngine.Rigidbody>();
        animator.SetInteger("animation", 34);
    }
    void Update()
    {
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


}
