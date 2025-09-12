using UnityEngine;

public class StunEffect : MonoBehaviour
{
    public MapObject effectData;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.enabled = false;
                Debug.Log($"{effectData.name} 발동 → {effectData.description}");
                Invoke(nameof(Recover), effectData.duration);
            }
        }
    }

    void Recover()
    {
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (player != null) player.enabled = true;
        Debug.Log($"{effectData.name} 종료");
    }

}
