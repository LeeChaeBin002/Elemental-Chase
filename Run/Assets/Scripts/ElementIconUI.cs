using UnityEngine;
using TMPro; // 텍스트 출력용
using UnityEngine.UI;
using System.Collections.Generic;

public class ElementIconUI : MonoBehaviour
{
    [Header("UI")]
    public Image elementIcon; // 선택된 원소 아이콘 표시할 UI
    public Button skillButton;  // 스킬 버튼 (아이콘 클릭 가능)

    private Color activeColor = Color.white;
    private Color inactiveColor = new Color(1f, 1f, 1f, 0.3f); // 반투명 회색

    // 외부에서 elementId 전달받아 세팅
    public void SetElementIcon(int elementId)
    {
        string iconName = "";
        switch (elementId)
        {
            case 1: iconName = "water"; break;
            case 2: iconName = "fire"; break;
            case 3: iconName = "air"; break;
            case 4: iconName = "earth"; break;
            default: iconName = "default"; break;
        }


        Sprite icon = Resources.Load<Sprite>($"ElementIcons/{iconName}");
        if (icon != null && elementIcon != null)
        {
            elementIcon.sprite = icon;
            Debug.Log($"[UI] {iconName} 아이콘 세팅 완료");
        }
        else
        {
            Debug.LogWarning($"아이콘 로드 실패: {iconName}");
        }
    }
    // 아이콘 활성화/비활성화
    public void SetSkillInteractable(bool canUse)
    {
        if (elementIcon != null)
            elementIcon.color = canUse ? activeColor : inactiveColor;

        if (skillButton != null)
            skillButton.interactable = canUse;
    }
}
