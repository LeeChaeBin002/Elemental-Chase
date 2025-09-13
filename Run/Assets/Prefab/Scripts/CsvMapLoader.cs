using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvMapLoader : MonoBehaviour
{
    // public GameObject prefab;
    public TextAsset csvFile;
    public List<MapObject> objects = new List<MapObject>();

    void Start()
    {
        string path = Path.Combine(Application.dataPath, "Prefab/MapObjects.csv");

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
            Debug.Log($"생성된 MapObject → name:{obj.name}, id:{obj.id}, buffId:{obj.buffId}"); // MapObject 생성 확인
            objects.Add(obj);


            // 🔑 Hierarchy 오브젝트 중 ObjectId.id 와 매칭
            foreach (var objId in FindObjectsByType<ObjectId>(FindObjectsSortMode.None))
            {
                Debug.Log($"Hierarchy 오브젝트: {objId.gameObject.name}, ObjectId:{objId.id}");

                if (objId.id == obj.id)
                {
                    var eff = objId.GetComponent<Effect>();
                    if (eff == null)
                        eff = objId.gameObject.AddComponent<Effect>();

                    eff.effectData = obj;
                    Debug.Log($"{obj.name}({obj.id}) → {objId.gameObject.name} 매칭 완료");
                }
            }

        }
    }
}
