using UnityEngine;
using System.Collections;

public class PlayerInitializer : MonoBehaviour
{
    public ElementIconUI iconUI;
    IEnumerator Start()
    {
        yield return null;
        if (ElementDataLoader.Instance == null)
        {
            Debug.LogError("ElementDataLoader.Instance가 없음!");
            yield break;
        }
        // 1. 랜덤 캐릭터 선택
        var (character, skills) = ElementDataLoader.Instance.GetRandomCharacterWithSkills();

        if (character == null || skills == null || skills.Count == 0)
            yield break;

        // 2. 스킬이 있다면 ElementId 가져오기
        int elementId = (skills != null && skills.Count > 0) ? skills[0].ElementId : 0;
       
        if (iconUI != null)
        {
            iconUI.SetElementIcon(elementId);
        }
        else
        {
            Debug.LogWarning("ElementIconUI 못찾음!");
        }

        // 4. PlayerMovement 반영
        PlayerMovement pm = GetComponent<PlayerMovement>(); // 자기 자신에 붙어 있는 PlayerMovement 가져오기
        if (pm != null)
        {
            pm.runSpeed = character.MoveSpeed;
            Debug.Log($"[PlayerInitializer] {character.Name} 적용됨 → 이동속도 {pm.runSpeed}");
        }
    }
}
