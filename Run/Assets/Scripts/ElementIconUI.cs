using System.Collections;
using System.Collections.Generic;
using TMPro; // 텍스트 출력용
using UnityEngine;
using UnityEngine.UI;

public class ElementIconUI : MonoBehaviour
{
    [Header("UI")]
    public Image elementIcon;
    public Button skillButton;  // 스킬 버튼 (아이콘 클릭 가능)
    [SerializeField] Image cooldownMask;     // 오버레이(위에서 만든 CooldownMask)
    [SerializeField] CanvasGroup cg;      // 버튼 차단/투명 조절용 (아이콘은 그대로)
    
    Coroutine cdRoutine;
    

    void Awake()
    {
        if (!cg) cg = GetComponent<CanvasGroup>();
        if (cg)
        {
            cg.alpha = 1f;               // 아이콘은 항상 보이게
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        if (cooldownMask)
        {
            cooldownMask.raycastTarget = false;
            cooldownMask.enabled = false;
            cooldownMask.fillAmount = 0f; // 0에서 시작(채워지도록)
                                          // 어둡게 덮을 색 (알파만 바꾸면 됨)
            cooldownMask.color = new Color(0f, 0f, 0f, 0.6f);
        }
        // 버튼 틴트가 아이콘을 희미하게 만드는 걸 방지
        if (skillButton)
        {
            var cb = skillButton.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
            cb.pressedColor = Color.white;
            cb.disabledColor = Color.white;    // 비활성이어도 아이콘 안사라지게
            skillButton.colors = cb;
        }
    }
    void Start()
    {
        if (skillButton)
        {
            skillButton.onClick.RemoveAllListeners();
            skillButton.onClick.AddListener(() =>
            {
                var player = FindObjectOfType<PlayerMovement>();
                if (player != null) player.UseSkill();
            });
        }
    }
    public void BeginCooldown(float seconds)
    {
        if (!gameObject.activeInHierarchy) return; // 꺼져있으면 코루틴 X
        if (cdRoutine != null) StopCoroutine(cdRoutine);
        cdRoutine = StartCoroutine(CooldownRoutine(seconds));
    }

    IEnumerator CooldownRoutine(float seconds)
    {
        // 5초 동안 잠금
        if (skillButton) skillButton.interactable = false;
        if (cg) cg.blocksRaycasts = false;
        // 오버레이 세팅: 아이콘과 동일한 스프라이트/크기, 회색 반투명
        if (cooldownMask)
        {
            // 아이콘과 동일 스프라이트로 덮어씌우면 더 자연스러움
            if (elementIcon) cooldownMask.sprite = elementIcon.sprite;
            cooldownMask.preserveAspect = true;
            cooldownMask.fillAmount = 0f;     // 0→1로 "차오르게"
            cooldownMask.enabled = true;
        }
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;                  // 타임스케일 영향 X
            if (cooldownMask) cooldownMask.fillAmount = 1f - (t / seconds);
            yield return null;
        }

        // 5초 후 해제
        if (skillButton) skillButton.interactable = true;
        if (cg) cg.blocksRaycasts = true;
        if (cooldownMask) { cooldownMask.enabled = false; cooldownMask.fillAmount = 0f; }

        cdRoutine = null;

    }
   
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


        var sprite = Resources.Load<Sprite>($"ElementIcons/{iconName}");
        if (sprite && elementIcon) elementIcon.sprite = sprite;
        if (elementIcon) elementIcon.sprite = sprite;
        if (cooldownMask) // 오버레이도 같은 스프라이트로
        {
            cooldownMask.sprite = sprite;
            cooldownMask.preserveAspect = true;
        }
    }
   



}
