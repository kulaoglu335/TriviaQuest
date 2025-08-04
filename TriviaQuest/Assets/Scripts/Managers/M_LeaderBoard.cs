using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class M_LeaderBoard : MonoBehaviour
{
    private LeaderboardData _currentData;
    
    [Header("Leaderboard UI")]
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotParent;
    [SerializeField] private int _slotsPerPage = 10;
    [SerializeField] private float _slotSpacing = 80f;
    [SerializeField] private Button _lbNextPageButton;
    [SerializeField] private Button _lbPreviousPageButton;
    [SerializeField] private TextMeshProUGUI _lbPageText;

    [SerializeField] private float slotMoveDuration;
    [SerializeField] private float slotMoveDelay;
    [SerializeField] private float slotHorizontalDistance;
    
    private List<UI_LeaderboardSlot> _spawnedSlots = new();
    private bool _isLastPage = false;
    private int _currentPage = 0;
    
    private string _leaderboardURL = string.Empty;

    private void Awake()
    {
        M_Game.I.RegisterLeaderBoard(this);
    }

    public void Init()
    {
        _lbNextPageButton.onClick.AddListener(OnClick_NextPage);
        _lbPreviousPageButton.onClick.AddListener(OnClick_PreviousPage);
        
        CreateSlots();
    }
    
    private void CreateSlots()
    {
        for (int i = 0; i < _slotsPerPage; i++)
        {
            GameObject obj = Instantiate(_slotPrefab, _slotParent);
            RectTransform rt = obj.GetComponent<RectTransform>();
            
            float spacing = -_slotSpacing;
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
        StartCoroutine(LoadLeaderboardPage(_currentPage,true,true,1,false,0));
    }

    private void OnClick_PreviousPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            StartCoroutine(LoadLeaderboardPage(_currentPage,true,true,2,false,0));
        }
    }
    #endregion

    #region Leaderboard Logic
    
    public void StartLeaderboard(string leaderboardURL,float delay)
    {
        _currentPage = 0;
        _leaderboardURL = leaderboardURL;
        StartCoroutine(LoadLeaderboardPage(_currentPage,false,true,1,true,delay));
    }

    public void FinishLeaderboard()
    {
        _currentPage = 0;
        _currentData = null;
    }
    
    private IEnumerator LoadLeaderboardPage(int page,bool closeAnimNeeded,bool openAnimNeeded, int leftOrRightPage,bool isFirstOpen,float delay)
    {
        //leftOrRight=1 means right page
        //leftOrRight=2 means left page
        
        yield return StartCoroutine(FetchLeaderboard(page));
        var data = _currentData;

        _isLastPage = data.is_last;
        UpdatePageText();

        if (isFirstOpen)
        {
            for (int i = 0; i < _spawnedSlots.Count; i++)
            {
                _spawnedSlots[i].gameObject.SetActive(false);
            }
        }
        
        int firstAnimType = 0;
        int secondAnimType = 0;
        if (closeAnimNeeded)
        {
            if (leftOrRightPage == 1) firstAnimType = 1;
            else if (leftOrRightPage == 2) firstAnimType = 3;
        }
        if (openAnimNeeded)
        {
            if (leftOrRightPage == 1) secondAnimType = 2;
            else if (leftOrRightPage == 2) secondAnimType = 4;
        }
        
        yield return new WaitForSeconds(delay);
        
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            StartCoroutine(_spawnedSlots[i].ApplyAnim(slotMoveDuration, i*slotMoveDelay, firstAnimType,secondAnimType,slotHorizontalDistance,data.data[i]));
        }
    }

    private void UpdatePageText()
    {
        if (_lbPageText != null)
            _lbPageText.text = (_currentPage + 1).ToString();
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
            catch (System.Exception ex)
            {
                _currentData = null;
            }
        }
        else
        {
            _currentData = null;
        }
    }

    #endregion
}
