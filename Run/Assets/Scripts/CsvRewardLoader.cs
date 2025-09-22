using UnityEngine;
using System.Collections.Generic;
using System.IO;
public class RewardData
{
    public string Name;          // 이름
    public int RewardID;         // 보상ID
    public int CurrencyType;
    public int ConditionType;    // 조건타입
    public float Threshold;      // 임계값
    public int Amount;           // 수량
    public string Note;          // 비고
};

public class ScoreData
{
    public string Name;          // 이름
    public int ScoreID;          // 스코어ID
    public int CurrencyType;
    public int Value;            // 값
    public string Description;   // 설명
}
public static class CsvRewardLoader
{
    private static Dictionary<string, string> rewardHeaderMap = new Dictionary<string, string>
    {
        {"보상 테이블", "Name"},
        {"보상ID", "RewardID"},
        {"재화종류", "CurrencyType"},
        {"조건타입", "ConditionType"},
        {"임계값", "Threshold"},
        {"수량", "Amount"},
        {"비고", "Note"}
    };
    private static Dictionary<string, string> scoreHeaderMap = new Dictionary<string, string>
    {
        {"스코어테이블", "Name"},
        {"스코어ID", "ScoreID"},
        {"값", "Value"},
        {"설명", "Description"}
    };
    public static List<T> LoadCSV<T>(TextAsset csvFile, Dictionary<string, string> headerMap) where T : new()
    {
        List<T> list = new List<T>();
        string[] lines = csvFile.text.Split('\n');

        if (lines.Length <= 1) return list;

        string[] headers = lines[0].Trim().Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Trim().Split(',');
            T obj = new T();

            var fields = typeof(T).GetFields();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                string header = headers[j].Trim();
                if (!headerMap.ContainsKey(header)) continue;

                string fieldName = headerMap[header];
                string value = values[j].Trim();

                foreach (var field in fields)
                {
                    if (field.Name == fieldName)
                    {
                        object converted = ConvertValue(field.FieldType, value);
                        //Debug.Log($"Header:{header}, Value:{value}, FieldName:{fieldName}");
                        field.SetValue(obj, converted);
                    }
                }
            }

            list.Add(obj);
        }
        return list;
    }

    private static object ConvertValue(System.Type type, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return type.IsValueType ? System.Activator.CreateInstance(type) : null;

        if (type == typeof(int))
        {
            int.TryParse(value, out int result);
            return result;
        }
        if (type == typeof(float))
        {
            float.TryParse(value, out float result);
            return result;
        }

        return value; // string
    }

    public static List<RewardData> LoadRewards(TextAsset csv) =>
        LoadCSV<RewardData>(csv, rewardHeaderMap);
    public static List<ScoreData> LoadScores(TextAsset csv) =>
         LoadCSV<ScoreData>(csv, scoreHeaderMap);
}
