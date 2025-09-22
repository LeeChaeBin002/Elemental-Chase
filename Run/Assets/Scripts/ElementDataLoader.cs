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
    public SkillTreeData SelectedTree => selectedTree;

    private int currentLevel = 1;  // 현재 구간/레벨
    private CharacterData selectedCharacter;
    private SkillTreeData selectedTree;

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


    // 🔹 현재 레벨에 해당하는 스킬 반환
    public (CharacterData, List<SkillData>) GetRandomCharacterWithSkillByLevel(int currentLevel = 1)
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
        if (tree == null) return (selected, null);
        // 현재 캐릭터 원소
        int elementId = tree.ElementId;

        // 3. 현재 레벨 유효성 검사
        if (currentLevel <= 0 || currentLevel > tree.Stages.Count)
        {
            Debug.LogWarning($"레벨 {currentLevel} 은 유효하지 않습니다.");
            return (selected, null);
        }

        // 4. 스킬트리에서 해당 레벨 스킬 ID 가져오기
        int skillId = tree.Stages[currentLevel - 1]; // 레벨1 → index0
        SkillData skill = skills.Find(s => s.SkillId == skillId);

        if (skill != null)
        {
            Debug.Log($"[스킬트리 기반 선택] Lv.{skill.Level} {skill.Name} - {skill.Description}");
            // List<SkillData>로 감싸서 반환
            return (selected, new List<SkillData> { skill });
        }
        else
        {
            Debug.LogWarning($"스킬 ID {skillId} 를 찾을 수 없습니다.");
            return (selected, null);
        }
    }

    void Start()
    {


    }

    // 🔹 현재 레벨에 해당하는 스킬 반환
    public (CharacterData, List<SkillData>) GetCharacterWithSkillByLevel(int level)
    {
        if (characters.Count == 0 || skillTrees.Count == 0 || skills.Count == 0)
            return (null, null);

        if (selectedCharacter == null)
            selectedCharacter = characters[UnityEngine.Random.Range(0, characters.Count)];

        if (selectedTree == null)
            selectedTree = skillTrees.Find(t => t.SkillTreeId == selectedCharacter.SkillTreeId);

        if (selectedTree == null || level <= 0 || level > selectedTree.Stages.Count)
            return (selectedCharacter, null);

        int skillId = selectedTree.Stages[level - 1];
        SkillData skill = skills.Find(s => s.SkillId == skillId);

        if (skill != null)
            return (selectedCharacter, new List<SkillData> { skill });

        return (selectedCharacter, null);

    }
    // 🔹 외부에서 호출해서 레벨 올리고 새 스킬 적용
    public void LevelUp()
    {
        currentLevel++;
        var (character, skillSet) = GetCharacterWithSkillByLevel(currentLevel);

        if (skillSet != null)
        {
            foreach (var skill in skillSet)
                Debug.Log($"[레벨업] Lv.{skill.Level} {skill.Name} - {skill.Description}");
        }
        else
        {
            Debug.Log($"[레벨업] {currentLevel} 단계 스킬 없음");
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

        //Debug.Log($"총 {characters.Count}개의 캐릭터 데이터 로드 완료");
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

        //Debug.Log($"총 {skillTrees.Count}개의 스킬트리 데이터 로드 완료");
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

       // Debug.Log($"총 {skills.Count}개의 스킬 데이터 로드 완료");
    }

    // 🔹 특정 원소와 레벨에 맞는 스킬 반환
    public SkillData GetSkillByElement(int elementId, int level)
    {
        // 스킬 찾기
        SkillData skill = skills.Find(s => s.ElementId == elementId && s.Level == level);

        if (skill != null)
            Debug.Log($"[원소 기반 선택] {skill.Name} (원소:{elementId}, Lv:{level})");
        else
            Debug.LogWarning($"[원소 기반 선택] {elementId}, Level:{level} 스킬을 찾을 수 없음");

        return skill;
    }
}


