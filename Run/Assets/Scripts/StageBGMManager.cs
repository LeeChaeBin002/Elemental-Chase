using UnityEngine;
using System.Collections;

public class StageBGMManager: MonoBehaviour
{
    public static StageBGMManager Instance;
    public AudioSource bgmSource;

    [Header("BGM 설정")]
    public AudioClip defaultBGM;   // 1~2구간 기본 BGM
    public AudioClip bossBGM;      // 3구간(보스) 전용 BGM

    [Header("페이드 설정")]
    public float fadeDuration = 1.5f; // 페이드 인/아웃 시간

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (bgmSource == null)
            bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
    }

    private void Start()
    {
        // 게임 시작 시 1구간 기본 BGM 실행
        if (defaultBGM != null)
        {
            PlayStageBGM(defaultBGM);
            Debug.Log("[StageBGMManager] 기본 BGM 시작");
        }
    }

    // 🔹 즉시 재생
    public void PlayStageBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return; // 같은 곡이면 무시

        bgmSource.clip = clip;
        bgmSource.volume = 1f;
        bgmSource.Play();
        Debug.Log("[StageBGMManager] BGM 실행: " + clip.name);
    }

    // 🔹 기존 → 새 곡으로 부드럽게 전환
    public void PlayStageBGMWithFade(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return; // 중복 방지

        StartCoroutine(FadeBGMCoroutine(clip));
    }

    private IEnumerator FadeBGMCoroutine(AudioClip newClip)
    {
        float startVolume = bgmSource.volume;
        float time = 0f;

        // 1️⃣ 현재 곡 페이드 아웃
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            yield return null;
        }
        bgmSource.Stop();

        // 2️⃣ 새로운 곡 재생 + 페이드 인
        bgmSource.clip = newClip;
        bgmSource.Play();

        time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }
        bgmSource.volume = 1f;

        Debug.Log($"[StageBGMManager] 페이드 전환 완료: {newClip.name}");
    }

    // 🔹 골인/게임오버 시 서서히 끄기
    public void StopBGMWithFade()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = bgmSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = 1f;
        Debug.Log("[StageBGMManager] 페이드 아웃 후 BGM 중지됨");
    }
}
