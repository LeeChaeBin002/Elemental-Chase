using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;   // 코인 프리팹
    public int coinCount = 50;      // 생성할 개수
 
    public GameObject road;

    void Start()
    {
        SpawnCoins();
    }

    void SpawnCoins()
    {
        
        // 길의 범위 가져오기
        BoxCollider box = road.GetComponent<BoxCollider>();
        Bounds bounds = box.bounds;

        // 레일 X 좌표를 3등분해서 정의
        float roadWidth = bounds.size.x;
        float leftLaneX = bounds.center.x - roadWidth / 3f;   // 왼쪽
        float middleLaneX = bounds.center.x;                  // 가운데
        float rightLaneX = bounds.center.x + roadWidth / 3f;  // 오른쪽
        float[] lanes = new float[] { leftLaneX, middleLaneX, rightLaneX };

        for (int i = 0; i < coinCount; i++)
        {
            // 레일 중 하나 랜덤 선택
            float x = lanes[Random.Range(0, lanes.Length)];

            // Z는 길 범위 안에서 랜덤
            float z = Random.Range(bounds.min.z, bounds.max.z);

            // Y는 길 위로 살짝 띄우기
            float y = bounds.max.y + 1f;

            Vector3 pos = new Vector3(x, y, z);

            // 코인 회전은 랜덤 (Y축만 돌리면 자연스러움)
            Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 90);

            // 코인 생성
            Instantiate(coinPrefab, pos, rot);
        }
    }
}
