using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvMapLoader : MonoBehaviour
{
    public GameObject prefab;
    public List<MapObject> objects = new List<MapObject>();

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "MapObjects.csv");

        if (!File.Exists(path))
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);

        // 첫 줄은 헤더라서 i=1부터
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(',');

            MapObject obj = new MapObject
            {
                name = cols[0],
                id = int.Parse(cols[1]),
                shakeOnHit = cols[2].ToLower() == "true",
                target = int.Parse(cols[3]),
                triggerType = int.Parse(cols[4]),
                duration = float.Parse(cols[5]),
                buffId = int.Parse(cols[6]),
                stateId = int.Parse(cols[7]),
                description = cols[8]
            };

            objects.Add(obj);
        

            // 오브젝트 생성
            Vector3 pos = new Vector3(i * 3, 0, 0);
            var go = Instantiate(prefab, pos, Quaternion.identity);
            go.name = obj.name;

            // ID 기반으로 효과 스크립트 붙이기
            if (obj.buffId == 324020 || obj.buffId == 324040)
            {
                var eff = go.AddComponent<SlowEffect>();
                eff.effectData = obj;
            }
            else if (obj.stateId == 31110)
            {
                var eff = go.AddComponent<StunEffect>();
                eff.effectData = obj;
            }
            else if (obj.buffId == 0 && obj.stateId == 0)
            {
                var eff = go.AddComponent<DamageEffect>();
                eff.effectData = obj;
            }
        }
    }
}
