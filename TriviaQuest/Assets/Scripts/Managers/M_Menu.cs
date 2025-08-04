using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class M_Menu : MonoBehaviour
{
    [Header("Menu Items")] 
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private UI_MenuButton playButton;
    [SerializeField] private UI_MenuButton leaderBoardButton;
    [SerializeField] private RectTransform leftSafePlaceTarget;
    public float buttonsOpenAnimDuration;
    public float buttonsCloseAnimDuration;
    public float buttonsDelayDuration;

    [Header("Leaderboard Items")]
    [SerializeField] private Button lbOpenButton;
    [SerializeField] private Button lbCloseButton;
    [SerializeField] private GameObject lbCanvas;
    [SerializeField] private GameObject lbPanel;
    public float lbPanelOpenCloseAnimDuration;
    private Tween _lbPanelTween;

    [Header("Quest Items")] 
    [SerializeField] private Button qOpenButton;
    [SerializeField] private Button qCloseButton;
    [SerializeField] private GameObject qCanvas;
    [SerializeField] private GameObject qPanel;
    
    public Action OnLeaderboardOpenClicked;
    public Action OnLeaderboardCloseClicked;
    public Action OnQuestOpenClicked;
    public Action OnQuestCloseClicked;
    private void Awake()
    {
        M_Game.I.RegisterMenu(this);
    }

    public void Init()
    {
        lbOpenButton.onClick.AddListener(() => OnLeaderboardOpenClicked?.Invoke());
        lbCloseButton.onClick.AddListener(() => OnLeaderboardCloseClicked?.Invoke());
        qOpenButton.onClick.AddListener(() => OnQuestOpenClicked?.Invoke());
        qCloseButton.onClick.AddListener(() => OnQuestCloseClicked?.Invoke());
        
        playButton.Init(buttonsOpenAnimDuration,buttonsCloseAnimDuration,0, leftSafePlaceTarget);
        leaderBoardButton.Init(buttonsOpenAnimDuration,buttonsCloseAnimDuration,buttonsDelayDuration, leftSafePlaceTarget);
        
        SetReadyMenuUI();
    }

    private void SetReadyMenuUI()
    {
        lbCanvas.SetActive(false);
        lbPanel.SetActive(false);
        qCanvas.SetActive(false);
        qPanel.SetActive(false);
        menuCanvas.SetActive(true);
        
        playButton.OpenEffect();
        leaderBoardButton.OpenEffect();
    }
    
    public IEnumerator OpenLeaderboardUI()
    {
        playButton.CloseEffect();
        leaderBoardButton.CloseEffect();

        yield return new WaitForSeconds(buttonsCloseAnimDuration);
        
        lbPanel.SetActive(true);
        lbCanvas.SetActive(true);
        
        if(_lbPanelTween != null) _lbPanelTween.Kill();
        lbPanel.transform.localScale = Vector3.zero;
        _lbPanelTween = lbPanel.transform.DOScale(Vector3.one, lbPanelOpenCloseAnimDuration).SetEase(Ease.OutBack);
    }

    public IEnumerator CloseLeaderboardUI()
    {
        if(_lbPanelTween != null) _lbPanelTween.Kill();
        lbPanel.transform.localScale = Vector3.one;
        _lbPanelTween = lbPanel.transform.DOScale(Vector3.zero, lbPanelOpenCloseAnimDuration).SetEase(Ease.InBack);
        
        yield return new WaitForSeconds(lbPanelOpenCloseAnimDuration);
        
        lbPanel.SetActive(false);
        lbCanvas.SetActive(false);
        
        playButton.OpenEffect();
        leaderBoardButton.OpenEffect();
    }

    public IEnumerator OpenQuestUI()
    {
        playButton.CloseEffect();
        leaderBoardButton.CloseEffect();

        yield return new WaitForSeconds(buttonsCloseAnimDuration + buttonsDelayDuration);
        
        qPanel.SetActive(true);
        qCanvas.SetActive(true);
        menuCanvas.SetActive(false);
    }
    
    public void CloseQuestUI()
    {
        SetReadyMenuUI();
    }
}
