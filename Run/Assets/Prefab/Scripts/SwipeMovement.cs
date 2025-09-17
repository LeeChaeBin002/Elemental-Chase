using UnityEngine;

public class SwipeMovement : MonoBehaviour
{
    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    private PlayerMovement playerMovement;

    void Start()
    {
        // 같은 오브젝트에 붙어있는 PlayerMovement 가져오기
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            startTouchPos = Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            endTouchPos = Input.mousePosition;
            DetectSwipe();
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                startTouchPos = touch.position;

            if (touch.phase == TouchPhase.Ended)
            {
                endTouchPos = touch.position;
                DetectSwipe();
            }
        }
#endif
    }

    void DetectSwipe()
    {
        Vector2 swipe = endTouchPos - startTouchPos;

        if (swipe.magnitude < 50f) return; // 너무 짧으면 무시

        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if (swipe.x > 0) playerMovement.ChangeLane(1);  // 오른쪽 이동
            else playerMovement.ChangeLane(-1);             // 왼쪽 이동
        }
    }
}





