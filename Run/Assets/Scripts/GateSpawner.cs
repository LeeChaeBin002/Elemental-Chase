using UnityEngine;

public class GateSpawner : MonoBehaviour
{

    [Header("이전 적")]
    public GameObject oldEnemy;


    [Header("새로 활성화할 적")]
    public GameObject newEnemy;

    [Header("구간 설정")]
    public int stageIndex;         // 이 게이트가 담당하는 구간 번호 (1, 2, 3…)

    [Header("스테이지 팝업 오브젝트")]
    public GameObject stage2Popup;
    public GameObject bossPopup;

    [Header("구간 전용 BGM")]
    public AudioClip stageBGM;            // 이 구간 전용 BGM

    [Header("이 구간 전용 UI 오브젝트")]
    public GameObject stagePopup;

    private bool hasSpawned = false; // 중복 스폰 방지

    private void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;
        if (other.CompareTag("Player"))
        {
            // 1️⃣ 이전 적 비활성화
            if (oldEnemy != null && oldEnemy.activeSelf)
            {
                oldEnemy.SetActive(false);
            }

            // 2️⃣ 다음 적 활성화
            if (newEnemy != null && !newEnemy.activeSelf)
            {
                newEnemy.SetActive(true);
                Debug.Log($"{gameObject.name} 게이트 통과 → {newEnemy.name} 활성화!");
                // 3️⃣ BGM 처리
                if (StageBGMManager.Instance != null)
                {
                    if (stageIndex <= 2)
                    {
                        // 1,2구간은 같은 음악 공유 → 이미 같은 클립이면 다시 안 틀기
                        if (StageBGMManager.Instance.bgmSource.clip != stageBGM)
                        {
                            StageBGMManager.Instance.PlayStageBGM(stageBGM);
                        }
                    }
                    else
                    {
                        // 3구간은 보스 BGM으로 페이드 전환
                        StageBGMManager.Instance.PlayStageBGMWithFade(StageBGMManager.Instance.bossBGM);
                    }
                }
            }
            // 4️⃣ UI 팝업
            if (StagePopupManager.Instance != null)
            {
     
                if (stageIndex == 2) StagePopupManager.Instance.ShowStage(1); // Stage2
                else if (stageIndex == 3) StagePopupManager.Instance.ShowStage(2); // Boss
            }
            hasSpawned = true; // 중복 방지
        }
    }
}
