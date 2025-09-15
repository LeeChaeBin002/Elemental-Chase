using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 3f;
    private bool isMoving = true;

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
            Debug.Log("Enemy reached the goal and stopped.");
        }
    }
}
