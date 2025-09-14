using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;   // 코인 프리팹
    public int coinCount = 50;      // 생성할 개수
    public Vector3 spawnArea = new Vector3(20, 0, 20); // 스폰 범위 (X,Z 랜덤)

    void Start()
    {
        SpawnCoins();
    }

    void SpawnCoins()
    {
        for (int i = 0; i < coinCount; i++)
        {
            // 랜덤 위치 구하기
            float x = Random.Range(-spawnArea.x, spawnArea.x);
            float z = Random.Range(-spawnArea.z, spawnArea.z);

            Vector3 pos = new Vector3(x, 1.0f, z);

            Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 90);

            // 코인 생성
            Instantiate(coinPrefab, pos, rot);
        }
    }
}
