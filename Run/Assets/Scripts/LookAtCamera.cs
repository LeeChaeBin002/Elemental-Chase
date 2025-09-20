using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam != null)
        {
            // 체력바가 항상 카메라를 정면으로 바라보게
            transform.forward = mainCam.transform.forward;
        }
    }
}
