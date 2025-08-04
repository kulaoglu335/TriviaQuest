using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class QuestUIHandler : MonoBehaviour
{
    [Header("UI References")]
    public List<UI_AnswerButton> answerButtons;
    public RectTransform leftSafePlaceTarget;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private UI_Category categoryUI;
    [SerializeField] private UI_Timer timerUI;
    [SerializeField] private UI_Score scoreUI;
    [SerializeField] private RectTransform topPanel;
    [SerializeField] private RectTransform questionPanel;
    [SerializeField] private RectTransform scorePanel;
    [SerializeField] private GameObject endMainMenuButton;
    [SerializeField] private Transform scoreBarEndPosTarget;
    private Vector3 _topPanelStartPosition;
    private Vector3 _questionPanelStartPosition;
    private Vector3 _scorePanelStartPosition;
    
    [Header("Feedback Text References")]
    [SerializeField] private List<string> correctAnswerStrings;
    [SerializeField] private List<string> incorrectAnswerStrings;
    [SerializeField] private TextMeshProUGUI correctAnswerEffectText;
    [SerializeField] private TextMeshProUGUI incorrectAnswerEffectText;
    private Tween _answerEffectScaleTween;
    private Tween _answerEffectFadeTween;

    [Header("Colors")]
    [SerializeField] private Color32 normalAnswerColor;
    [SerializeField] private Color32 correctAnswerColor;
    [SerializeField] private Color32 incorrectAnswerColor;

    private void Start()
    {
        _topPanelStartPosition = topPanel.anchoredPosition;
        _questionPanelStartPosition = questionPanel.anchoredPosition;
        _scorePanelStartPosition = scorePanel.anchoredPosition;
    }
    
    public void StartQuestUIs()
    {
        topPanel.anchoredPosition = _topPanelStartPosition;
        questionPanel.anchoredPosition = _questionPanelStartPosition;
        scorePanel.anchoredPosition = _scorePanelStartPosition;
        endMainMenuButton.SetActive(false);
        scoreUI.ResetScore();
    }

    public void FinishQuestUIs()
    {
        topPanel.transform.DOLocalMove(new Vector3(0, 500, 0), 0.5f).SetRelative(true);
        questionPanel.transform.DOLocalMove(new Vector3(leftSafePlaceTarget.localPosition.x,0,0), 0.5f).SetRelative(true);
        scorePanel.transform.DOMove(scoreBarEndPosTarget.transform.position, 0.5f);
        
        endMainMenuButton.transform.localScale = Vector3.zero;
        endMainMenuButton.SetActive(true);
        endMainMenuButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    
    public void SetQuestionData(QuestionData questionData,Action timerOnCompleteCallback,int currentIndex,int totalQuestionAmount,float questionDuration)
    {
        questionText.text = questionData.question;
        categoryUI.SetCategoryTo(questionData.category);
        timerUI.StartTimer(questionDuration,timerOnCompleteCallback);
        scoreUI.SetQuestionNumber(currentIndex+1,totalQuestionAmount);

        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].SetButton(questionData.choices[i]);
            answerButtons[i].SetColor(normalAnswerColor);
        }
        
        correctAnswerEffectText.gameObject.SetActive(false);
        incorrectAnswerEffectText.gameObject.SetActive(false);
    }

    public void StopTimer()
    {
        timerUI.StopTimer();
    }
    
    public void HighlightAnswer(int index, bool correct)
    {
        var color = correct ? correctAnswerColor : incorrectAnswerColor;
        answerButtons[index].SetColor(color);
    }

    public void OpenButtonUIs()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].OpenEffect();
        }
    }

    public void CloseButtonUIs()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].CloseEffect();
        }
    }
    
    public void CorrectEffect(int newScore)
    {
        int randomIndex = UnityEngine.Random.Range(0, correctAnswerStrings.Count);
        correctAnswerEffectText.text = correctAnswerStrings[randomIndex];
        
        correctAnswerEffectText.transform.localScale = Vector3.zero;
        Color c = correctAnswerEffectText.color;
        c.a = 1.0f;
        correctAnswerEffectText.color = c;
        correctAnswerEffectText.gameObject.SetActive(true);
        if(_answerEffectScaleTween != null) _answerEffectScaleTween.Kill();
        if(_answerEffectFadeTween != null) _answerEffectFadeTween.Kill();
        _answerEffectScaleTween = correctAnswerEffectText.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack);
        _answerEffectFadeTween = correctAnswerEffectText.DOFade(0, 0.5f).SetDelay(1f);
        
        scoreUI.UpdateScore(newScore,true);
    }
    
    public void IncorrectEffect(int newScore)
    {
        int randomIndex = UnityEngine.Random.Range(0, incorrectAnswerStrings.Count);
        incorrectAnswerEffectText.text = incorrectAnswerStrings[randomIndex];
        
        incorrectAnswerEffectText.transform.localScale = Vector3.zero;
        Color c = incorrectAnswerEffectText.color;
        c.a = 1.0f;
        incorrectAnswerEffectText.color = c;
        incorrectAnswerEffectText.gameObject.SetActive(true);
        if(_answerEffectScaleTween != null) _answerEffectScaleTween.Kill();
        if(_answerEffectFadeTween != null) _answerEffectFadeTween.Kill();
        _answerEffectScaleTween = incorrectAnswerEffectText.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack);
        _answerEffectFadeTween = incorrectAnswerEffectText.DOFade(0, 0.5f).SetDelay(1f);
        
        scoreUI.UpdateScore(newScore, false);
    }
}
