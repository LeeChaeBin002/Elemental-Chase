using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StagePopupManager : MonoBehaviour
{

    public static StagePopupManager Instance;

    [Header("스테이지 팝업 오브젝트들 (Stage1~Boss 순서대로)")]
    public GameObject[] stagePopups;   // 0=Stage1, 1=Stage2, 2=Boss...

    [Header("페이드 설정")]
    public float fadeDuration = 1f;
    public float stayDuration = 1.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < stagePopups.Length; i++)
        {
            if (stagePopups[i] != null)
            {
                // index 0(Stage1)은 켜둔다
                stagePopups[i].SetActive(i == 0);
            }
        }
    }

    /// <summary>
    /// 스테이지 인덱스로 팝업 호출 (0=Stage1, 1=Stage2, 2=Boss...)
    /// </summary>
    public void ShowStage(int index)
    {
        if (index < 0 || index >= stagePopups.Length)
        {
            Debug.LogWarning($"[StagePopup] 잘못된 index {index}, 배열 크기 {stagePopups.Length}");
            return;
        }

        GameObject popup = stagePopups[index];
        Debug.Log($"[StagePopup] ShowStage 호출됨 → {popup.name}");
        StartCoroutine(ShowPopupRoutine(popup));
    }
    public IEnumerator ShowStageRoutineForCountdown(int index)
    {
        if (index < 0 || index >= stagePopups.Length)
        {
            Debug.LogWarning($"[StagePopup] 잘못된 index {index}, 배열 크기 {stagePopups.Length}");
            yield break;
        }

        GameObject popup = stagePopups[index];
        Debug.Log($"[StagePopup] ShowStageRoutineForCountdown 호출됨 → {popup.name}");
        yield return StartCoroutine(ShowPopupRoutine(popup));
    }

    private IEnumerator ShowPopupRoutine(GameObject popup)
    {
        popup.SetActive(true);

        CanvasGroup cg = popup.GetComponent<CanvasGroup>();
        if (cg == null) cg = popup.AddComponent<CanvasGroup>();

        // 페이드 인
        cg.alpha = 0f;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        cg.alpha = 1f;

        yield return new WaitForSeconds(stayDuration);

        // 페이드 아웃
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }
        cg.alpha = 0f;

        popup.SetActive(false);
        Debug.Log($"[StagePopup] {popup.name} → 페이드 아웃 완료 후 비활성화");
    }


}


