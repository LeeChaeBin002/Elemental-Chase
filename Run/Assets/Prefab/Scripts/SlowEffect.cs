using UnityEngine;

public class SlowEffect : MonoBehaviour
{
    public MapObject effectData;
    private float originalSpeed;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                originalSpeed = pm.runSpeed;
                // 예: 버프 ID별로 감소율 다르게
                if (effectData.buffId == 324020) pm.runSpeed *= 0.8f; // 20% 감소
                else if (effectData.buffId == 324040) pm.runSpeed *= 0.6f; // 40% 감소

                Debug.Log($"{effectData.name} 발동 → {effectData.description}");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null) pm.runSpeed = originalSpeed;
            Debug.Log($"{effectData.name} 종료");
        }
    }
}
