using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public RewardUI rewardUI;
    
    public GameObject loseUI;

    public ElementDataLoader elementDataLoader;
    public CharacterData selectedCharacter;

    [Header("스테이지 관리")]
    public int currentStage = 1;
    public Transform[] playerSpawnPoints; // 구간별 플레이어 위치
    public Transform[] enemySpawnPoints;  // 구간별 적 위치
    private PlayerMovement player;
    private EnemyMove enemy;

    public List<CharacterData> characters => elementDataLoader.characters;
    void Awake()
    {
        PickRandomCandidates(4);
        // SelectOneFromCandidates();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (loseUI != null)
            loseUI.SetActive(false);
    }

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        enemy = FindObjectOfType<EnemyMove>();
        rewardUI.gameObject.SetActive(true);   // 켜둔 뒤
        rewardUI.rewardParent.SetActive(false); // 내부 패널만 꺼두기
        MoveToStage(currentStage);
    }
    public void MoveToStage(int stage)
    {
        currentStage = stage;

        if (playerSpawnPoints.Length >= stage && player != null)
        {
            player.transform.position = playerSpawnPoints[stage - 1].position;
            player.enabled = true;
        }

        if (enemySpawnPoints.Length >= stage && enemy != null)
        {
            enemy.transform.position = enemySpawnPoints[stage - 1].position;
            enemy.SetStunned(false); // 혹시 스턴 풀기
        }

        Debug.Log($"[GameManager] {stage} 구간 시작!");
    }
    public void NextStage(int prevStage)
    {
        int nextStage = prevStage + 1;

        if (nextStage <= playerSpawnPoints.Length)
            MoveToStage(nextStage);
        else
            Debug.Log("[GameManager] 마지막 스테이지 클리어!");
    }
    public void ShowLoseUI()
    {
        if (loseUI != null)
            loseUI.SetActive(true);
        Time.timeScale = 0f; // 게임 정지
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 중복 방지

        // 플레이어 찾아서 바닥 위로 보정
        var player = FindAnyObjectByType<PlayerMovement>();
        var respawn = GameObject.FindWithTag("Respawn");

        if (player != null && respawn != null)
        {
            player.RespawnAt(respawn.transform);
        }
    }
    public void ShowRewardUI()
    {
        Time.timeScale = 0f; // 게임 멈춤
        if (rewardUI != null)
        {
            rewardUI.gameObject.SetActive(true);  // 🔹 반드시 켜주기
            rewardUI.ShowReward();
            Debug.Log("[GameManager] 보상 UI 실행됨");
        }
        else
        {
            Debug.LogWarning("[GameManager] rewardUI가 Inspector에 연결되지 않음!");
        }
    }
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
   

    void PickRandomCandidates(int count)
    {
    
    }
}
