using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class M_Menu : MonoBehaviour
{
    public static M_Menu I { get; private set; }

    [Header("Leaderboard UI")]
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotParent;
    [SerializeField] private int _slotsPerPage = 10;
    [SerializeField] private float _slotSpacing = 80f;

    [SerializeField] private Button _lbOpenButton;
    [SerializeField] private Button _lbCloseButton;
    [SerializeField] private Button _lbNextPageButton;
    [SerializeField] private Button _lbPreviousPageButton;
    [SerializeField] private GameObject _lbCanvas;
    [SerializeField] private GameObject _lbPanel;
    [SerializeField] private TextMeshProUGUI _lbPageText;

    private List<UI_LeaderboardSlot> _spawnedSlots = new();
    private M_LeaderBoard _leaderBoard;
    private bool _isLastPage = false;
    private int _currentPage = 0;
    
    //[Header("Question UI")]
    

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        _lbOpenButton.onClick.AddListener(OnClick_OpenLeaderboard);
        _lbCloseButton.onClick.AddListener(OnClick_CloseLeaderboard);
        _lbNextPageButton.onClick.AddListener(OnClick_NextPage);
        _lbPreviousPageButton.onClick.AddListener(OnClick_PreviousPage);

        _lbCanvas.SetActive(false);
        _lbPanel.SetActive(false);
        CreateSlots();
    }

    public void Init(M_LeaderBoard lb)
    {
        _leaderBoard = lb;
    }

    #region UI Button Events

    private void OnClick_OpenLeaderboard()
    {
        _currentPage = 0;
        StartCoroutine(LoadLeaderboardPage(_currentPage));
    }

    private void OnClick_CloseLeaderboard()
    {
        _lbPanel.SetActive(false);
        _lbCanvas.SetActive(false);
    }

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
        yield return StartCoroutine(_leaderBoard.FetchLeaderboard(page));
        var data = _leaderBoard.GetLatestData();

        _isLastPage = data.is_last;

        UpdateLeaderboardUI(data);
        UpdatePageText();
        _lbCanvas.SetActive(true);
        _lbPanel.SetActive(true);
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

    #endregion
}
