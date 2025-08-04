using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class QuestUIHandler : MonoBehaviour
{
    [Header("UI References")]
    public List<UI_AnswerButton> answerButtons;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private UI_Category categoryUI;
    [SerializeField] private UI_Timer timerUI;
    [SerializeField] private UI_Score scoreUI;
    
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
        
        OpenButtonUIs();
    }

    public void StopTimer()
    {
        timerUI.StopTimer();
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
    
    public void HighlightAnswer(int index, bool correct)
    {
        var color = correct ? correctAnswerColor : incorrectAnswerColor;
        answerButtons[index].SetColor(color);
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
        
        scoreUI.UpdateScore(newScore);
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
        
        scoreUI.UpdateScore(newScore);
    }
}
