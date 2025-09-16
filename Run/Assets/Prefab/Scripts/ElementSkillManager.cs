using UnityEngine;
using System.Collections.Generic;
public class SkillData //ElementSkill.csv
{
    public string Name;
    public int SkillId;
    public int ElementId;
    public int Level;
    public int TargetType;
    public float Duration;
    public float CoolTime;
    public float Damage;
    public float Attack;
    public int BuffId1;
    public int BuffId2;
    public string Description;
    // 호환용 별칭
    public string Desc => Description;
}
public class SkillTreeData//SkillTree.csv
{
    public string Name;
    public int SkillTreeId;
    public int ElementId;
    public int[] SkillIds; // {1단계, 2단계, 3단계, 4단계}
}
public class CharacterData //Charater.csv
{
    public string Name;
    public int CharacterId;
    public float MoveSpeed;
    public int SkillTreeId;
}

public class ElementSkillManager : MonoBehaviour
{
    public CharacterData character;        // 캐릭터 테이블에서 로드
    public SkillTreeData skillTree;        // 스킬 트리 데이터
    public List<SkillData> skills = new(); // 현재 스킬들

    private int currentWave = 1;
    private SkillData currentSkill; // 현재 웨이브에서 사용할 스킬

    public void Init(CharacterData charData, ElementSkillManager loader)
    {
        character = charData;

        // 스킬 트리 찾기
        //skillTree = loader.skillTrees.Find(t => t.SkillTreeId == character.SkillTreeId);
        if (skillTree == null)
        {
            Debug.LogError($"스킬 트리 {character.SkillTreeId}를 찾을 수 없음");
            return;
        }

        skills.Clear();
        foreach (var skillId in skillTree.SkillIds)
        {
            var skill = loader.skills.Find(s => s.SkillId == skillId);
            if (skill != null) skills.Add(skill);
        }

        Debug.Log($"캐릭터 {character.CharacterId} → 스킬 {skills.Count}개 로드 완료");
        UpdateCurrentSkill();
    }

    public void NextWave()
    {
        currentWave++;
        Debug.Log($"웨이브 {currentSkill.Description} 시작 → {currentWave}단계 스킬까지 사용 가능");
        UpdateCurrentSkill();
    }
    private void UpdateCurrentSkill()
    {
        currentSkill = skills.Find(s => s.Level == currentWave);
        if (currentSkill != null)
        {
            Debug.Log($"웨이브 {currentWave}: 현재 스킬 = {currentSkill.Desc} (ID {currentSkill.SkillId})");
        }
        else
        {
            Debug.LogWarning($"웨이브 {currentWave}에 해당하는 스킬이 없음!");
        }
    }
    public void UseSkill(int level)
    {
        if (level > currentWave) // 아직 해금 안 됨
        {
            Debug.LogWarning($"{level}단계 스킬은 아직 잠겨있습니다!");
            return;
        }

        var skill = skills.Find(s => s.Level == level);
        if (skill != null)
        {
            Debug.Log($"스킬 사용: {skill.Desc} (데미지 {skill.Damage}, 쿨타임 {skill.CoolTime})");
            // TODO: 실제 공격 / 버프 적용 로직 추가
        }
    }
}
