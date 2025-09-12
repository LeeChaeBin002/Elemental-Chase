using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    public MapObject effectData;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                // description 보고 % 추출하는 대신, ID 규칙을 만들면 더 안정적
                int damage = Mathf.CeilToInt(hp.maxHp * 0.05f); // 예: 5%
                hp.TakeDamage(damage);

                Debug.Log($"{effectData.name} 발동 → {effectData.description}");
            }
        }
    }
}
