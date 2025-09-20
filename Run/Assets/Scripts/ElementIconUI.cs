using UnityEngine;
using TMPro; // 텍스트 출력용
using UnityEngine.UI;
using System.Collections.Generic;

public class ElementIconUI : MonoBehaviour
{
    [Header("UI")]
    public Image elementIcon; // 선택된 원소 아이콘 표시할 UI

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

}
