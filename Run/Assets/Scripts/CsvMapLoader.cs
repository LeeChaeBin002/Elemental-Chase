using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;   // Android에서 필요
using System.Collections;      // IEnumerator 사용

public class CsvMapLoader : MonoBehaviour
{
    public string relativePath = "Prefab/CSV/MapObjects.csv";
    public List<MapObject> objects = new List<MapObject>();

    void Start()
    {
        StartCoroutine(LoadCsv());
    }

    private IEnumerator LoadCsv()
    {
        string path = Path.Combine(Application.streamingAssetsPath, relativePath);
        string[] lines = null;

#if UNITY_ANDROID && !UNITY_EDITOR
        // ✅ Android는 UnityWebRequest 필요
        UnityWebRequest req = UnityWebRequest.Get(path);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string csvText = req.downloadHandler.text;
            lines = csvText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogError("CSV 로드 실패: " + req.error);
            yield break;
        }
#else
        //  Windows, macOS, Editor
        if (File.Exists(path))
        {
            lines = File.ReadAllLines(path);
        }
        else
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + path);
            yield break;
        }
#endif

        //  CSV 파싱 (이 부분은 원래 코드 그대로 유지)
        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더
        {
            string[] cols = lines[i].Split(',');

            if (cols.Length < 9) continue; // 안전장치

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

            Debug.Log($"생성된 MapObject → name:{obj.name}, id:{obj.id}, buffId:{obj.buffId}");
            objects.Add(obj);

            // 🔹 Hierarchy 오브젝트 매칭
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