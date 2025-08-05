using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_Game : MonoBehaviour
{
    public static M_Game I { get; private set; }

    [Header("Q/A Configs")] 
    [SerializeField] private int correctScore;
    [SerializeField] private int incorrectScore;
    [SerializeField] private int timeoutScore;
    [SerializeField] private float questionDuration;

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
        
        SceneManager.LoadScene(1);
    }

    #region Dependencies

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
        _quest.Init(correctScore, incorrectScore, timeoutScore, questionDuration);
        
        _menu.OnQuestOpenClicked = OpenQuest;
        _menu.OnQuestCloseClicked = CloseQuest;
    }

    #endregion

    #region Open/Close

    private void OpenLeaderBoard()
    {
        _leaderBoard.StartLeaderboard(GetLeaderboardUrl(),_menu.buttonsCloseAnimDuration);
        StartCoroutine(_menu.OpenLeaderboardUI());
    }

    private void CloseLeaderBoard()
    {
        _leaderBoard.FinishLeaderboard();
        StartCoroutine(_menu.CloseLeaderboardUI());
    }

    private void OpenQuest()
    {
        _quest.StartQuest(questionsURL,_menu.buttonsCloseAnimDuration + _menu.buttonsDelayDuration);
        StartCoroutine(_menu.OpenQuestUI());
    }

    private void CloseQuest()
    {
        _menu.CloseQuestUI();
    }

    #endregion
    public string GetLeaderboardUrl()
    {
        #if UNITY_EDITOR
            return leaderboardURLFormatEditor;
        #else
            return leaderboardURLFormatBuild;
        #endif
    }
}
