using UnityEngine;

public class SlopeStick : MonoBehaviour
{
    private Rigidbody rb;
    private bool isActive = false;

    [Header("Raycast Settings")]
    public float rayDistance = 5f;   // 아래로 쏠 거리
    public LayerMask groundMask;     // 경사면 포함된 레이어

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        // 캐릭터 중심에서 아래로 레이 쏘기
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayDistance, groundMask))
        {
            Vector3 pos = rb.position;
            pos.y = hit.point.y + 0.01f;   // 바닥 살짝 위에 붙이기
            rb.MovePosition(pos);
        }
    }

    public void EnableStick(bool active)
    {
        isActive = active;
        if (!active && rb != null)
        {
            // Zone 나오면 원래 물리에 맡기기
            rb.isKinematic = false;
        }
    }
}
