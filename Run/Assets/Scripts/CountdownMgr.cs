using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountdownMgr : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public GameObject gameplayUI;

    public PlayerMovement player;  
    public EnemyMove[] enemies;    

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

        countdownText.gameObject.SetActive(true);

        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        countdownText.text = "Start!";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.gameObject.SetActive(false);

        if (restartScene)
        {
            // 🔹 씬 리로드
            isCountingDown = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            if (gameplayUI != null) gameplayUI.SetActive(true);

            // 🔹 캐릭터/적 다시 켜기
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
}
