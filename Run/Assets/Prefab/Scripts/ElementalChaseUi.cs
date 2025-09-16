using UnityEngine;
using UnityEngine.SceneManagement;

public class ElementalChaseUI : MonoBehaviour
{
    // 게임 씬 이름 (에디터에서 할당 가능)
    public string gameSceneName = "GameScene";

    void Update()
    {
        // 아무 키나 눌리면
        if (Input.anyKeyDown)
        {
            StartGame();
        }
    }
    void StartGame()
    {
        Debug.Log("게임 시작!");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickExit()
    {
        Debug.Log("게임 종료");
        Application.Quit();

        // 에디터에서 실행 중일 때도 종료되게
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
