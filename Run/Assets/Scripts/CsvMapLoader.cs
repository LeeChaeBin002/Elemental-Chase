using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvMapLoader : MonoBehaviour
{
    // public GameObject prefab;
    public string resourcePath = "CSV/MapObjects"; // Resources/CSV/MapObjects.csv
    public List<MapObject> objects = new List<MapObject>();

    void Start()
    {
        //Resources에서 TextAsset 불러오기
        TextAsset csvFile = Resources.Load<TextAsset>(resourcePath);

        if (csvFile == null)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: Resources/{resourcePath}.csv");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);


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
            //Debug.Log($"생성된 MapObject → name:{obj.name}, id:{obj.id}, buffId:{obj.buffId}"); // MapObject 생성 확인
            objects.Add(obj);


            // Hierarchy 오브젝트 중 ObjectId.id 와 매칭
            foreach (var objId in FindObjectsByType<ObjectId>(FindObjectsSortMode.None))
            {
                //Debug.Log($"Hierarchy 오브젝트: {objId.gameObject.name}, ObjectId:{objId.id}");

                if (objId.id == obj.id)
                {
                    var eff = objId.GetComponent<Effect>();
                    if (eff == null)
                        eff = objId.gameObject.AddComponent<Effect>();

                    eff.effectData = obj;
                    //Debug.Log($"{obj.name}({obj.id}) → {objId.gameObject.name} 매칭 완료");
                }
            }

        }
    }

    public MapObject GetMapObjectById(int id)
    {
        foreach (var obj in objects)
        {
            if (obj.id == id)
                return obj;
        }

        Debug.LogWarning($"MapObject ID {id}를 찾을 수 없습니다!");
        return null;
    }
}
