using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountdownMgr : MonoBehaviour
{
    public PlayerMovement player;
    public EnemyMove[] enemies;
    public GameObject gameplayUI;

    [Header("Stage UI")]
    public CanvasGroup stageGroup;     // 페이드 효과
    public float fadeDuration = 1f;
    public float stayDuration = 1.5f;
    [SerializeField] private GameObject startStageText; // ⬅️ 시작할 때만 보여줄 자식 오브젝트

    private bool isCountingDown = false;
    void Start()
    {
        // 게임 시작할 때 자동 카운트다운
        BeginCountdown(false);
    
    }
    public void BeginCountdown(bool restartScene = false)
    {
        if (isCountingDown) return;
        StartCoroutine(StartCountdown(restartScene));
    }

    IEnumerator StartCountdown(bool restartScene)
    {
        isCountingDown = true;

        if (gameplayUI != null) gameplayUI.SetActive(false);

        // 🔹 캐릭터/적 멈추기
        if (player != null)
        {
            player.enabled = false;
            player.animator.SetInteger("animation", 34); // Idle 강제
        }

        if (enemies != null)
        {
            foreach (var e in enemies)
            {
                if (e == null) continue;
                e.enabled = false;
                e.animator.SetInteger("animation", 34); // Idle 강제
            }
        }
        // 🔹 Stage 1 텍스트 (StagePopupManager 활용)
        if (StagePopupManager.Instance != null)
            yield return StagePopupManager.Instance.ShowStageRoutineForCountdown(0); // index 0 = Stage1


        if (restartScene)
        {
            isCountingDown = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            if (gameplayUI != null) gameplayUI.SetActive(true);

            if (player != null) player.enabled = true;
            if (enemies != null)
            {
                foreach (var e in enemies)
                {
                    if (e == null) continue;
                    e.enabled = true;
                }
            }
        }

        isCountingDown = false;
    }
    public IEnumerator ShowStageText(GameObject textObj)
    {
        // StageUI(Canvas)는 항상 켜둠
        if (!stageGroup.gameObject.activeSelf)
            stageGroup.gameObject.SetActive(true);

        textObj.SetActive(true);
        stageGroup.alpha = 0;   // 처음은 투명하게 시작
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            stageGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(stayDuration);

        t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            stageGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        textObj.SetActive(false);
        stageGroup.alpha = 0; // 투명하게만 만들기
    }

}
