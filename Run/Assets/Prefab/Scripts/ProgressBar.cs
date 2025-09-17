using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Transform player;
    public Transform startPoint;
    public Collider goalTrigger;   // 골 트리거 콜라이더
    public Slider slider;

    private float startZ;
    private float goalEndZ;
    private float totalDistance;
    private float playerHalfDepth; // 플레이어 크기 보정

    void Start()
    {
        startZ = startPoint.position.z;

        // 골 트리거 끝
        goalEndZ = goalTrigger.bounds.min.z;

        // 플레이어 앞부분 보정
        Collider playerCol = player.GetComponent<Collider>();
        playerHalfDepth = (playerCol != null) ? playerCol.bounds.size.z * 0.5f : 0f;

        totalDistance = goalEndZ - startZ;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;

    }

    void Update()
    {
        // 플레이어 중심 + 절반 크기 = 실제 "앞부분"
        float playerFrontZ = player.position.z + playerHalfDepth;

        float traveled = playerFrontZ - startZ;
        float progress = Mathf.Clamp01(traveled / totalDistance);

        slider.value = progress*100f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other == goalTrigger)
        {
            // 골 트리거에 닿는 순간 강제로 100%
            slider.value = 100f;
         
        }
    }
}
