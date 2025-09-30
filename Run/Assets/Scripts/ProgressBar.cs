using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour
{
    public Transform player;
    public Transform[] sectionPoints;   // 0=시작, 1=중간1, 2=중간2, 3=골
    public Slider slider;
    public Image[] sectionIcons;        // UI 아이콘 (0,1,2,골 아이콘)

    private float totalDistance;
    private bool[] sectionReached;
    void OnEnable()
    {
       
        if (slider != null)
            slider.value = 0f;
    }
    void Start()
    {
        totalDistance = sectionPoints[sectionPoints.Length - 1].position.z
                        - sectionPoints[0].position.z;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;

        sectionReached = new bool[sectionPoints.Length];

        // 아이콘은 항상 불투명하게 표시
        foreach (var icon in sectionIcons)
            icon.color = Color.white;

    }

    void Update()
    {
        float playerZ = player.position.z;

        // 현재 플레이어가 속한 구간 찾기
        for (int i = 0; i < sectionPoints.Length - 1; i++)
        {
            float startZ = sectionPoints[i].position.z;
            float endZ = sectionPoints[i + 1].position.z;

            if (playerZ >= startZ && playerZ <= endZ)
            {
                // 구간 내 진행도
                float localProgress = (playerZ - startZ) / (endZ - startZ);

                // 전체 구간에서의 비율 계산
                float sectionStartRatio = (float)i / (sectionPoints.Length - 1);
                float sectionEndRatio = (float)(i + 1) / (sectionPoints.Length - 1);

                float progress = Mathf.Lerp(sectionStartRatio, sectionEndRatio, localProgress);
                slider.value = progress;
                //  구간 처음 돌입 시 아이콘 효과 실행
                if (!sectionReached[i])
                {
                    sectionReached[i] = true;
                    StartCoroutine(IconPopEffect(sectionIcons[i].transform));
                }


                break;
            }
        }
        // 골 지점 넘어가면 1.0 고정 + 마지막 아이콘 효과
        if (playerZ >= sectionPoints[sectionPoints.Length - 1].position.z)
        {
            slider.value = 1f;

            int last = sectionPoints.Length - 1;
            if (!sectionReached[last])
            {
                sectionReached[last] = true;
                StartCoroutine(IconPopEffect(sectionIcons[last].transform));
            }
        }
    }
    IEnumerator IconPopEffect(Transform icon)
    {
        Vector3 originalScale = icon.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float duration = 0.2f;
        float time = 0f;

        // 커지기
        while (time < duration)
        {
            icon.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        icon.localScale = targetScale;

        // 돌아오기
        time = 0f;
        while (time < duration)
        {
            icon.localScale = Vector3.Lerp(targetScale, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        icon.localScale = originalScale;
    }
}

