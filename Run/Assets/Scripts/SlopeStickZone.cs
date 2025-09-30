using UnityEngine;

public class SlopeStickZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            var pm = other.GetComponent<PlayerMovement>();
            if (pm != null) pm.SetSlopeZone(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
    if (other.CompareTag("Player"))
    {
            var pm = other.GetComponent<PlayerMovement>();
            if (pm != null) pm.SetSlopeZone(false);
        }
    }
}
