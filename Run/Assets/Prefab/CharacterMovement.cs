using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
   
    private ElementSkillManager skillManager;

    void Start()
    {
        var charData = GameManager.Instance.selectedCharacter;
        skillManager = gameObject.AddComponent<ElementSkillManager>();
       // skillManager.Init(charData, GameManager.Instance.csvLoader);
    }

    void Update()
    {
        // 스킬 발동 (Space 키 예시)
        if (Input.GetKeyDown(KeyCode.Space))
            //skillManager.UseSkill();

        // 웨이브 진행 테스트
        if (Input.GetKeyDown(KeyCode.N))
            skillManager.NextWave();
    }
}
