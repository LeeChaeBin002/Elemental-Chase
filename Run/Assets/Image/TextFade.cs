using TMPro;
using UnityEngine;

public class TextFade : MonoBehaviour
{
    public TMP_Text text;           // 할당할 Text
    public float fadeDuration = 1f; // 페이드 인/아웃 시간
    public bool loop = true;        // 반복 여부

    private float timer = 0f;
    private bool fadingOut = true;

    void Update()
    {
        if (text == null) return;

        timer += Time.deltaTime / fadeDuration;

        // 알파 값 계산 (0 ↔ 1 반복)
        float alpha = fadingOut ? 1 - timer : timer;

        Color c = text.color;
        c.a = Mathf.Clamp01(alpha);
        text.color = c;

        if (timer >= 1f)
        {
            timer = 0f;
            fadingOut = !fadingOut; // 방향 반전
            if (!loop && !fadingOut) enabled = false; // 루프 끄면 한번만 실행
        }
    }
}
