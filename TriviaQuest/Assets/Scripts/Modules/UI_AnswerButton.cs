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
        transform.localScale = Vector3.zero;
    }

    public void InitButton(int index, Action<int> onClickCallback, float openEffectDuration, float closeEffectDuration, float delay)
    {
        _answerIndex = index;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(_answerIndex));
        _openEffectDuration = openEffectDuration;
        _closeEffectDuration = closeEffectDuration;
        _delay = delay;
    }

    public void SetButton(string answerTextString)
    {
        answerText.text = answerTextString;
    }

    public void SetColor(Color32 color)
    {
        buttonImage.color = color;
    }

    public void OpenEffect()
    {
        transform.localScale = Vector3.zero;
        //transform.localPosition = _startPosition;
        if(_openTween != null) _openTween.Complete();
        _openTween = transform.DOScale(Vector3.one, _openEffectDuration).SetEase(Ease.OutBack).SetDelay(_answerIndex * _delay);
    }

    public void CloseEffect()
    {
        transform.localScale = Vector3.one;
        //transform.localPosition = _startPosition;
        if(_closeTween != null) _closeTween.Complete();
        _closeTween = transform.DOLocalMove(new Vector3(-1000,0,0),_closeEffectDuration).SetRelative(true).SetEase(Ease.InCubic).SetDelay(_answerIndex * _delay);
    }
}
