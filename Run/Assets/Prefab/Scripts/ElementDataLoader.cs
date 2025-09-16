using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class ElementData
{
    public string Name;
    public int Id;
    public string Type;
    public string Desc;
}
public class ElementDataLoader : MonoBehaviour
{
    public List<SkillData> skills = new List<SkillData>();
    public List<SkillTreeData> skillTrees = new List<SkillTreeData>();
    public List<CharacterData> characters = new List<CharacterData>();

    public string skillCsvFile = "CSV/ElementSkill.csv";  
    public string skillTreeCsvFile = "SCV/SkillTrees.csv";  
    public string characterCsvFile = "SCV/Characters.csv";  

    void Awake()
    {
        LoadCSV();
    }

    void LoadCSV()
    {
       //// string path = Path.Combine(Application.streamingAssetsPath, fileName);
       // if (!File.Exists(path))
       // {
       //     Debug.LogError($"CSV 파일을 찾을 수 없음: {path}");
       //     return;
       // }

       // string[] lines = File.ReadAllLines(path);
       // for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더라서 1부터 시작
       // {
       //     string[] cols = lines[i].Split(',');
       //     ElementData data = new ElementData
       //     {
       //         Name = cols[0],
       //         Id = int.Parse(cols[1]),
       //         Type = cols[2],
       //         Desc = cols[3]
       //     };
       //     elements.Add(data);
       // }
    }
}
