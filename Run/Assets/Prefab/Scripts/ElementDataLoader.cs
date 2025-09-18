using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class CharacterData
{
    public int CharacterId;
    public float MoveSpeed;
    public int SkillTreeId;
    public string Name;
    public int Id;
    public string Type;
    public string Desc;
}
public class ElementDataLoader : MonoBehaviour
{
    public static ElementDataLoader Instance { get; private set; }

    //public List<SkillData> skills = new List<SkillData>();
    //public List<SkillTreeData> skillTrees = new List<SkillTreeData>();
    public List<CharacterData> characters = new List<CharacterData>();

    public string skillCsvFile = "CSV/ElementSkill.csv";  
    public string skillTreeCsvFile = "SCV/SkillTrees.csv";  
    public string characterCsvFile = "SCV/Characters.csv";  

    void Awake()
    {   
        // 🔹 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환해도 유지하고 싶으면 추가
        }
        else
        {
            Destroy(gameObject); // 중복 방지
            return;
        }
        LoadCSV();
    }

    void LoadCSV()
    {
        string path = Path.Combine(Application.streamingAssetsPath, characterCsvFile);
        if (!File.Exists(path))
        {
            Debug.LogError($"CSV 파일을 찾을 수 없음: {path}");
            return;
        }

        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더라서 1부터 시작
        {
            string[] cols = lines[i].Split(',');
            CharacterData data = new CharacterData
            {
                //Name = cols[0], // 이제 모호성 없음
                Id = int.Parse(cols[1]),
                Type = cols[2],
                Desc = cols[3]
            };
            characters.Add(data);
        }
    }

   
}
