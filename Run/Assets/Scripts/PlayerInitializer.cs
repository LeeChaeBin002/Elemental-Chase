using UnityEngine;
using System.Collections;

public class PlayerInitializer : MonoBehaviour
{
    public ElementIconUI iconUI;
    void Start()
    {
        var loader = ElementDataLoader.Instance;
        if (loader == null) return;

        // 선택된 캐릭터/스킬 가져오기
        var (character, skillSet) = loader.GetCharacterWithSkillByLevel(1); // 현재 레벨 1단계
        if (character == null || skillSet == null || skillSet.Count == 0) return;

        var skill = skillSet[0];

        // PlayerMovement에 정확히 세팅
        var pm = GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.runSpeed = character.MoveSpeed;
            pm.SetSkill(skill);

            // 기존 코드에서 loader.selectedTree.ElementId 대신 loader.SelectedTree.ElementId로 변경
            Debug.Log($"[PlayerInitializer] {character.Name} (원소:{loader.SelectedTree.ElementId}) 스킬 적용됨 → {skill.Name}");
         
        }

        // UI 아이콘 세팅 (Inspector에 할당된 걸 사용)
        if (iconUI != null)
            iconUI.SetElementIcon(skill.ElementId);
    }
}
