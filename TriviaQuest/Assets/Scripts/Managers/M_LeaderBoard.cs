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
    
    #region UI Button Events
    private void OnClick_NextPage()
    {
        if (_isLastPage) return;
        
        _currentPage++;
        StartCoroutine(LoadLeaderboardPage(_currentPage));
    }

    private void OnClick_PreviousPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            StartCoroutine(LoadLeaderboardPage(_currentPage));
        }
    }
    #endregion

    #region Leaderboard Logic

    public void StartLeaderboard(string leaderboardURL)
    {
        _currentPage = 0;
        _leaderboardURL = leaderboardURL;
        StartCoroutine(LoadLeaderboardPage(_currentPage));
    }

    public void FinishLeaderboard()
    {
        _currentPage = 0;
        _currentData = null;
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
            obj.SetActive(false);
        }
    }
    
    private IEnumerator LoadLeaderboardPage(int page)
    {
        yield return StartCoroutine(FetchLeaderboard(page));
        var data = _currentData;

        _isLastPage = data.is_last;

        UpdateLeaderboardUI(data);
        UpdatePageText();
    }

    private void UpdateLeaderboardUI(LeaderboardData data)
    {
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            if (i < data.data.Length)
            {
                _spawnedSlots[i].SetData(data.data[i]);
                _spawnedSlots[i].gameObject.SetActive(true);
            }
            else
            {
                _spawnedSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdatePageText()
    {
        if (_lbPageText != null)
            _lbPageText.text = $"Page {_currentPage + 1}";
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
                Debug.Log($"Leaderboard page {page} fetched successfully!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("JSON Parse Error: " + ex.Message);
                _currentData = null;
            }
        }
        else
        {
            Debug.LogError("Leaderboard fetch failed: " + request.error);
            _currentData = null;
        }
    }

    #endregion
}
