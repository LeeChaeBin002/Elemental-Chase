using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject loseUI;

    public ElementDataLoader elementDataLoader;
    public CharacterData selectedCharacter;

    public List<SkillData> skills => elementDataLoader.skills;
    public List<SkillTreeData> skillTrees => elementDataLoader.skillTrees;
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
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
   

    void PickRandomCandidates(int count)
    {
    //    candidateElementIds.Clear();

    //    if (elementDataLoader == null || elementDataLoader.elements.Count == 0)
    //    {
    //        Debug.LogError("CSV 데이터를 불러오지 못했습니다!");
    //        return;
    //    }

    //    for (int i = 0; i < count; i++)
    //    {
    //        int index = Random.Range(0, elementDataLoader.elements.Count);
    //        int pickedId = elementDataLoader.elements[index].Id;
    //        candidateElementIds.Add(pickedId);
    //    }

    //    Debug.Log("랜덤 원소 후보 4명: " + string.Join(", ", candidateElementIds));
    //}
    //void SelectOneFromCandidates()
    //{
    //    int index = Random.Range(0, candidateElementIds.Count);
    //    currentElementId = candidateElementIds[index];

    //    Debug.Log($"최종 선택된 원소 ID = {currentElementId}");
    }
}
