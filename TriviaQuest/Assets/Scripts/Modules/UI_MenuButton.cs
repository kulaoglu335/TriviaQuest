using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class UI_MenuButton : MonoBehaviour
{
    private Vector3 _startPosition;
    private Tween _openTween;
    private Tween _closeTween;
    
    private float _openEffectDuration;
    private float _closeEffectDuration;
    private float _delay;
    
    private RectTransform _rectTransform;
    
    public void Init(float openEffectDuration, float closeEffectDuration, float delay)
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPosition = _rectTransform.anchoredPosition;
        transform.localScale = Vector3.zero;
        
        _openEffectDuration = openEffectDuration;
        _closeEffectDuration = closeEffectDuration;
        _delay = delay;
    }
    
    public void OpenEffect()
    {
        transform.localScale = Vector3.zero;
        _rectTransform.anchoredPosition = _startPosition;
        if(_openTween != null) _openTween.Complete();
        _openTween = transform.DOScale(Vector3.one, _openEffectDuration).SetEase(Ease.OutBack).SetDelay(_delay);
    }

    public void CloseEffect()
    {
        transform.localScale = Vector3.one;
        _rectTransform.anchoredPosition = _startPosition;
        if(_closeTween != null) _closeTween.Complete();
        _closeTween = transform.DOLocalMove(new Vector3(-1000,0,0),_closeEffectDuration).SetRelative(true).SetEase(Ease.InCubic).SetDelay(_delay);
    }
}
