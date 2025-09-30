using UnityEngine;

public class SlopeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.SetSlopeZone(true);
                //other.attachedRigidbody.useGravity = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.SetSlopeZone(false);
                //other.attachedRigidbody.useGravity = true;
            }
        }
    }
}
