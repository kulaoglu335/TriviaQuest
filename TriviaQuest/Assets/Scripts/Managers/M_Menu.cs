using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class M_Menu : MonoBehaviour
{
    [Header("Menu Items")] 
    [SerializeField] private GameObject menuCanvas;

    [Header("Leaderboard Items")]
    [SerializeField] private Button lbOpenButton;
    [SerializeField] private Button lbCloseButton;
    [SerializeField] private GameObject lbCanvas;
    [SerializeField] private GameObject lbPanel;

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
        //qCloseButton.onClick.AddListener(() => OnQuestCloseClicked?.Invoke());
        
        lbCanvas.SetActive(false);
        lbPanel.SetActive(false);
        qCanvas.SetActive(false);
        qPanel.SetActive(false);
        menuCanvas.SetActive(true);
    }
    
    public void OpenLeaderboardUI()
    {
        lbPanel.SetActive(true);
        lbCanvas.SetActive(true);
    }

    public void CloseLeaderboardUI()
    {
        lbPanel.SetActive(false);
        lbCanvas.SetActive(false);
    }

    public void OpenQuestUI()
    {
        qPanel.SetActive(true);
        qCanvas.SetActive(true);
    }
    
    public void CloseQuestUI()
    {
        qPanel.SetActive(false);
        qCanvas.SetActive(false);
    }
}
