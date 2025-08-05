using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class M_LeaderBoard : MonoBehaviour
{
    private LeaderboardData _currentData;
    public LeaderboardUIHandler uiHandler;
    
    [Header("Leaderboard UI")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private Button lbNextPageButton;
    [SerializeField] private Button lbPreviousPageButton;
    [SerializeField] private float slotSpacing;
    [SerializeField] private int slotsPerPage;

    [SerializeField] private float slotMoveDuration;
    [SerializeField] private float slotMoveDelay;
    [SerializeField] private float slotHorizontalDistance;
    
    private List<UI_LeaderboardSlot> _spawnedSlots = new();
    private bool _isLastPage = false;
    private int _currentPage = 0;
    
    private string _leaderboardURL = string.Empty;
    private enum PageDirection { None, Right = 1, Left = 2 }

    private void Awake()
    {
        M_Game.I.RegisterLeaderBoard(this);
    }

    public void Init()
    {
        lbNextPageButton.onClick.AddListener(OnClick_NextPage);
        lbPreviousPageButton.onClick.AddListener(OnClick_PreviousPage);
        
        CreateSlots();
    }
    
    private void CreateSlots()
    {
        for (int i = 0; i < slotsPerPage; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            RectTransform rt = obj.GetComponent<RectTransform>();
            
            float spacing = -slotSpacing;
            Vector2 anchoredPos = new Vector2(0f, i * spacing);
            rt.anchoredPosition = anchoredPos;

            var slot = obj.GetComponent<UI_LeaderboardSlot>();
            _spawnedSlots.Add(slot);
        }
    }
    
    #region UI Button Events
    private void OnClick_NextPage()
    {
        if (_isLastPage) return;
        
        _currentPage++;
        StartCoroutine(LoadLeaderboardPage(_currentPage,true,true,PageDirection.Right,false,0));
    }

    private void OnClick_PreviousPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            StartCoroutine(LoadLeaderboardPage(_currentPage,true,true,PageDirection.Left,false,0));
        }
    }
    #endregion

    #region Leaderboard Logic
    
    public void StartLeaderboard(string leaderboardURL,float delay)
    {
        _currentPage = 0;
        _leaderboardURL = leaderboardURL;
        StartCoroutine(LoadLeaderboardPage(_currentPage,false,true,PageDirection.Right,true,delay));
    }

    public void FinishLeaderboard()
    {
        _currentPage = 0;
        _currentData = null;
    }
    
    private IEnumerator LoadLeaderboardPage(int page, bool closeAnimNeeded, bool openAnimNeeded, PageDirection direction, bool isFirstOpen, float delay)
    {
        yield return StartCoroutine(FetchLeaderboard(page));
        var data = _currentData;

        _isLastPage = data.is_last;
        uiHandler.UpdatePageText(_currentPage);

        if (isFirstOpen) DeactivateAllSlots();
        
        var (firstAnimType, secondAnimType) = DetermineAnimTypes(direction, closeAnimNeeded, openAnimNeeded);
        
        yield return new WaitForSeconds(delay);
        
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            StartCoroutine(_spawnedSlots[i].ApplyAnim(
                slotMoveDuration, 
                i*slotMoveDelay,
                firstAnimType,
                secondAnimType,
                slotHorizontalDistance,
                data.data[i]));
        }
    }
    
    private IEnumerator FetchLeaderboard(int page = 0)
    {
        string url = string.Format(_leaderboardURL, page);
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                _currentData = JsonUtility.FromJson<LeaderboardData>(request.downloadHandler.text);
            }
            catch
            {
                _currentData = null;
            }
        }
        else
        {
            _currentData = null;
        }
    }
    
    private void DeactivateAllSlots()
    {
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            _spawnedSlots[i].gameObject.SetActive(false);
        }
    }
    
    private (int firstAnim, int secondAnim) DetermineAnimTypes(PageDirection direction, bool closeNeeded, bool openNeeded)
    {
        int firstAnim = 0, secondAnim = 0;

        if (closeNeeded)
        {
            firstAnim = direction switch
            {
                PageDirection.Right => 1,
                PageDirection.Left => 3,
                _ => 0
            };
        }

        if (openNeeded)
        {
            secondAnim = direction switch
            {
                PageDirection.Right => 2,
                PageDirection.Left => 4,
                _ => 0
            };
        }

        return (firstAnim, secondAnim);
    }

    #endregion
}
