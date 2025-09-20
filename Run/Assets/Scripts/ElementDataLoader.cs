using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class CharacterData
{
    public string Name;       // 캐릭터 이름
    public int CharacterId;   // 캐릭터 ID
    public float MoveSpeed;   // 이동 속도
    public int SkillTreeId;   // 연결된 스킬트리 ID
}

public class SkillTreeData
{
    public string Name;       // 스킬트리 이름
    public int SkillTreeId;   // 스킬트리 고유 ID
    public int ElementId;     // 원소 ID
    public List<int> Stages;  // 각 단계별 스킬 ID
}
public class SkillData
{
    public string Name;       // 스킬 이름
    public int SkillId;       // 스킬 ID
    public int ElementId;     // 원소 ID
    public int Level;         // 스킬 레벨
    public int TargetType;    // 타겟 타입 (1: 자기 자신, 2: 적 등)
    public float Duration;    // 지속 시간
    public float CoolTime;    // 쿨타임
    public int Damage;        // 피해량
    public int Attack;        // 공격 계수
    public int BuffId1;       // 버프 ID 1
    public int BuffId2;       // 버프 ID 2
    public string Description;// 설명
}
public class ElementDataLoader : MonoBehaviour
{
    public static ElementDataLoader Instance { get; private set; }

    public List<CharacterData> characters = new List<CharacterData>();
    public List<SkillTreeData> skillTrees = new List<SkillTreeData>();
    public List<SkillData> skills = new List<SkillData>();


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCharacters();
            LoadSkillTrees();
            LoadSkills();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔹 캐릭터 선택 + 해당 스킬 세트 가져오기
    public (CharacterData, List<SkillData>) GetRandomCharacterWithSkills()
    {
        if (characters.Count == 0 || skillTrees.Count == 0 || skills.Count == 0)
        {
            Debug.LogError("데이터가 충분히 로드되지 않았습니다.");
            return (null, null);
        }

        // 1. 랜덤 캐릭터 선택
        CharacterData selected = characters[UnityEngine.Random.Range(0, characters.Count)];
        Debug.Log($"[선택된 캐릭터] {selected.Name} (ID:{selected.CharacterId}, 속도:{selected.MoveSpeed}, 스킬트리:{selected.SkillTreeId})");

        // 2. 해당 캐릭터의 스킬트리 찾기
        SkillTreeData tree = skillTrees.Find(t => t.SkillTreeId == selected.SkillTreeId);
        if (tree == null)
        {
            Debug.LogError($"스킬트리 {selected.SkillTreeId} 를 찾을 수 없습니다.");
            return (selected, null);
        }

        // 3. 스킬트리에 포함된 스킬 ID → 실제 SkillData 변환
        List<SkillData> skillSet = new List<SkillData>();
        foreach (int skillId in tree.Stages)
        {
            SkillData skill = skills.Find(s => s.SkillId == skillId);
            if (skill != null)
            {
                skillSet.Add(skill);
            }
            else
            {
                Debug.LogWarning($"스킬 ID {skillId} 를 찾을 수 없습니다.");
            }
        }

        Debug.Log($"[스킬 세트] {tree.Name} → {skillSet.Count}개 스킬 로드됨");

        return (selected, skillSet);
    }

    void Start()
    {
        var (character, skillSet) = ElementDataLoader.Instance.GetRandomCharacterWithSkills();

        if (character != null)
        {
            Debug.Log($"선택된 캐릭터: {character.Name}");
            // 플레이어 이동 속도 반영
            var pm = FindObjectOfType<PlayerMovement>();
            if (pm != null)
            {
                pm.runSpeed = character.MoveSpeed;
            }
        }

        if (skillSet != null)
        {
            foreach (var skill in skillSet)
            {
                Debug.Log($"스킬: {skill.Name}, 설명: {skill.Description}");
            }
        }

        
    }

    void LoadCharacters()
    {
        TextAsset csvData = Resources.Load<TextAsset>("CSV/Character");
        if (csvData == null)
        {
            Debug.LogError("Character.csv 로드 실패");
            return;
        }

        string[] lines = csvData.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] cols = lines[i].Trim().Split(',');

            CharacterData data = new CharacterData
            {
                Name = cols[0],
                CharacterId = int.Parse(cols[1]),
                MoveSpeed = float.Parse(cols[2]),
                SkillTreeId = int.Parse(cols[3])
            };

            characters.Add(data);
        }

        Debug.Log($"총 {characters.Count}개의 캐릭터 데이터 로드 완료");
    }
    void LoadSkillTrees()
    {
        TextAsset csvData = Resources.Load<TextAsset>("CSV/SkillTree");
        if (csvData == null)
        {
            Debug.LogError("SkillTree.csv 로드 실패");
            return;
        }

        string[] lines = csvData.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] cols = lines[i].Trim().Split(',');

            SkillTreeData data = new SkillTreeData
            {
                Name = cols[0],
                SkillTreeId = int.Parse(cols[1]),
                ElementId = int.Parse(cols[2]),
                Stages = new List<int>
                {
                    int.Parse(cols[3]),
                    int.Parse(cols[4]),
                    int.Parse(cols[5]),
                    int.Parse(cols[6])
                }
            };

            skillTrees.Add(data);
        }

        Debug.Log($"총 {skillTrees.Count}개의 스킬트리 데이터 로드 완료");
    }
    void LoadSkills()
    {
        TextAsset csvData = Resources.Load<TextAsset>("CSV/ElementSkill");
        if (csvData == null)
        {
            Debug.LogError("ElementSkill.csv 로드 실패");
            return;
        }

        string[] lines = csvData.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] cols = lines[i].Trim().Split(',');

            SkillData data = new SkillData
            {
                Name = cols[0],
                SkillId = int.Parse(cols[1]),
                ElementId = int.Parse(cols[2]),
                Level = int.Parse(cols[3]),
                TargetType = int.Parse(cols[4]),
                Duration = float.Parse(cols[5]),
                CoolTime = float.Parse(cols[6]),
                Damage = int.Parse(cols[7]),
                Attack = int.Parse(cols[8]),
                BuffId1 = int.Parse(cols[9]),
                BuffId2 = int.Parse(cols[10]),
                Description = cols[11]
            };

            skills.Add(data);
        }

        Debug.Log($"총 {skills.Count}개의 스킬 데이터 로드 완료");
    }
}


