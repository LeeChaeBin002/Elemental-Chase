using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StoryMgr : MonoBehaviour
{
    [System.Serializable]
    public class StoryPage
    {
        public CanvasGroup background;
        public CanvasGroup[] subtitles;
    }

    public StoryPage[] pages;
    public float backgroundFadeDuration = 1f;   // 배경 전환 속도
    public float subtitleFadeDuration = 1.5f;   // 자막 전환 속도

    private int currentPage = 0;
    private int currentSubtitle = 0;
    private bool isFading = false;
    private bool skipFade = false; // 🔹 스킵 플래그

    void Start()
    {
        // 초기화
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i].background != null)
            {
                pages[i].background.alpha = (i == 0 ? 1 : 0);
                pages[i].background.gameObject.SetActive(i == 0);
            }

            foreach (var sub in pages[i].subtitles)
            {
                sub.alpha = 0;
                sub.gameObject.SetActive(false);
            }
        }

        // 첫 자막 시작
        ShowSubtitle(0, 0);
        StartCoroutine(FadeIn(pages[0].subtitles[0], subtitleFadeDuration));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isFading)
            {
                // 🔹 페이드 중 클릭 → 즉시 스킵
                skipFade = true;
            }
            else
            {
                StartCoroutine(NextSubtitleOrPage());
            }
        }
    }

    IEnumerator NextSubtitleOrPage()
    {
        isFading = true;

        // 현재 자막 페이드 아웃
        yield return StartCoroutine(FadeOut(pages[currentPage].subtitles[currentSubtitle], subtitleFadeDuration));

        currentSubtitle++;

        if (currentSubtitle < pages[currentPage].subtitles.Length)
        {
            ShowSubtitle(currentPage, currentSubtitle);
            yield return StartCoroutine(FadeIn(pages[currentPage].subtitles[currentSubtitle], subtitleFadeDuration));
        }
        else
        {
            foreach (var sub in pages[currentPage].subtitles)
                sub.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.3f);

            int prevPage = currentPage;
            currentPage++;
            currentSubtitle = 0;

            if (currentPage < pages.Length)
            {
                pages[currentPage].background.gameObject.SetActive(true);
                yield return StartCoroutine(CrossFadeBackground(pages[prevPage].background, pages[currentPage].background));

                ShowSubtitle(currentPage, 0);
                yield return StartCoroutine(FadeIn(pages[currentPage].subtitles[0], subtitleFadeDuration));
            }
            else
            {
                Debug.Log("스토리 끝! → 다음 씬 이동");
                yield return new WaitForSeconds(0.2f);
                SceneManager.LoadScene("Game1");
            }
        }

        isFading = false;
        skipFade = false;
    }

    IEnumerator CrossFadeBackground(CanvasGroup prev, CanvasGroup next)
    {
        float t = 0;
        next.alpha = 0;

        while (t < backgroundFadeDuration && !skipFade)
        {
            t += Time.deltaTime;
            float progress = t / backgroundFadeDuration;

            if (prev != null) prev.alpha = Mathf.Lerp(1, 0, progress);
            if (next != null) next.alpha = Mathf.Lerp(0, 1, progress);

            yield return null;
        }

        if (prev != null)
        {
            prev.alpha = 0;
            prev.gameObject.SetActive(false);
        }
        if (next != null) next.alpha = 1;
    }

    void ShowSubtitle(int pageIndex, int subtitleIndex)
    {
        var sub = pages[pageIndex].subtitles[subtitleIndex];
        sub.gameObject.SetActive(true);
        sub.alpha = 0;
    }

    IEnumerator FadeIn(CanvasGroup cg, float duration)
    {
        cg.gameObject.SetActive(true);
        cg.alpha = 0;
        float t = 0;

        while (t < duration && !skipFade)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }

        cg.alpha = 1;
    }

    IEnumerator FadeOut(CanvasGroup cg, float duration)
    {
        float t = 0;

        while (t < duration && !skipFade)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1, 0, t / duration);
            yield return null;
        }

        cg.alpha = 0;
        cg.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
