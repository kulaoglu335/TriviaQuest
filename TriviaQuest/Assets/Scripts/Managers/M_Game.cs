using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_Game : MonoBehaviour
{
    public static M_Game I { get; private set; }

    [Header("Q/A Configs")] 
    public int correctScore;
    public int incorrectScore;
    public int timeoutScore;

    [Header("Data Configs")]
    [SerializeField] private string leaderboardURLFormatEditor;
    [SerializeField] private string leaderboardURLFormatBuild;
    [SerializeField] private string questionsURL;

    private M_Menu _menu;
    private M_LeaderBoard _leaderBoard;
    private M_Quest _quest;
    
    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(1);
    }

    #region Registers

    public void RegisterMenu(M_Menu menu)
    {
        _menu = menu;
        _menu.Init();
        
        _menu.OnLeaderboardOpenClicked = OpenLeaderBoard;

        _menu.OnLeaderboardCloseClicked = CloseLeaderBoard;
    }

    public void RegisterLeaderBoard(M_LeaderBoard leaderBoard)
    {
        _leaderBoard = leaderBoard;
        _leaderBoard.Init();
    }
    
    public void RegisterQuest(M_Quest quest)
    {
        _quest = quest;
        _quest.Init();
        
        _menu.OnQuestOpenClicked = OpenQuest;

        _menu.OnQuestCloseClicked = CloseQuest;
    }

    #endregion

    #region Open/Close

    private void OpenLeaderBoard()
    {
#if UNITY_EDITOR
        string url = leaderboardURLFormatEditor;
#else
            string url = leaderboardURLFormatBuild;
#endif
        
        _leaderBoard.StartLeaderboard(url);
        _menu.OpenLeaderboardUI();
    }

    private void CloseLeaderBoard()
    {
        _leaderBoard.FinishLeaderboard();
        _menu.CloseLeaderboardUI();
    }

    private void OpenQuest()
    {
        _quest.StartQuest(questionsURL);
        _menu.OpenQuestUI();
    }

    private void CloseQuest()
    {
        _quest.FinishQuest();
        _menu.CloseQuestUI();
    }

    #endregion
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            
        }
    }
}
