using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    public float rotateSpeed = 180f; // 초당 회전 속도
    public int scoreValue = 5;      // 코인 점수


    void Update()
    {
        // Y축 기준으로 계속 회전
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ScoreManager에 점수 추가
            ScoreManager.instance.AddCoin();

            // 코인 제거
            Destroy(gameObject);
        }
    }
}
