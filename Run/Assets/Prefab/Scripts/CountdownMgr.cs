using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using JetBrains.Annotations;

public class CountdownMgr : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public GameObject gameplayUI;

    public PlayerMovement player;   // PlayerMovement 스크립트 직접 참조
    public EnemyMove enemy;

    void Start()
    {
        if (gameplayUI != null) gameplayUI.SetActive(false);

        // 움직임 스크립트 꺼두기
        if (player != null)
        {
            player.enabled = false;
            player.animator.SetInteger("animation", 34); // Idle 강제
        }

        if (enemy != null)
        {
            enemy.enabled = false;
            enemy.animator.SetInteger("animation", 34); // Idle 강제
        }

        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);

        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        countdownText.text = "Start!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        if (gameplayUI != null) gameplayUI.SetActive(true);

        // 움직임 다시 켜주기
        if (player != null) player.enabled = true;
        if (enemy != null) enemy.enabled = true;

        Debug.Log("게임 시작!");
    }
}
