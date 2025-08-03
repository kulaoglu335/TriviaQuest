using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_AnswerButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private Button button;
    
    private int _answerIndex = 0;
    private Color _normalAnswerColor;
    private Color _correctAnswerColor;
    private Color _incorrectAnswerColor;

    private Vector3 _startPosition;
    private Tween _openTween;
    private Tween _closeTween;

    private float _openEffectDuration;
    private float _closeEffectDuration;
    private float _delay;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        
        _startPosition = button.transform.localPosition;
    }

    public void SetButtonFuncs(int index, Action<int> onClickCallback, Color normalAnswerColor, Color correctAnswerColor, Color incorrectAnswerColor, float openEffectDuration, float closeEffectDuration, float delay)
    {
        _answerIndex = index;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(_answerIndex));
        _normalAnswerColor = normalAnswerColor;
        _correctAnswerColor = correctAnswerColor;
        _incorrectAnswerColor = incorrectAnswerColor;
        _openEffectDuration = openEffectDuration;
        _closeEffectDuration = closeEffectDuration;
        _delay = delay;
        
        buttonImage.color = _normalAnswerColor;
    }

    public void SetReady(string answerTextString)
    {
        answerText.text = answerTextString;
        buttonImage.color = _normalAnswerColor;
        OpenEffect();
    }

    public void CorrectAnswerEffect()
    {
        buttonImage.color = _correctAnswerColor;
    }

    public void IncorrectAnswerEffect()
    {
        buttonImage.color = _incorrectAnswerColor;
    }

    public void OpenEffect()
    {
        transform.localScale = Vector3.zero;
        transform.localPosition = _startPosition;
        if(_openTween != null) _openTween.Complete();
        _openTween = transform.DOScale(Vector3.one, _openEffectDuration).SetEase(Ease.OutBack).SetDelay(_answerIndex * _delay);
    }

    public void CloseEffect()
    {
        transform.localScale = Vector3.one;
        transform.localPosition = _startPosition;
        if(_closeTween != null) _closeTween.Complete();
        _closeTween = transform.DOLocalMove(new Vector3(-1000,0,0),_closeEffectDuration).SetRelative(true).SetEase(Ease.InCubic).SetDelay(_answerIndex * _delay);
    }
}
