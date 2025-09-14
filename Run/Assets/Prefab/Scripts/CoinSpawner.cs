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

        for (int i = 0; i < coinCount; i++)
        {
            // 랜덤 위치 구하기
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            float y = bounds.max.y + 1.0f; // 길 위 0.5 띄우기

            Vector3 pos = new Vector3(x, y, z);

            Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 90);

            // 코인 생성
            Instantiate(coinPrefab, pos, rot);
        }
    }
}
