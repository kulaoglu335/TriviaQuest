using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class UI_Timer : MonoBehaviour
{
    [SerializeField] private Image timerFillImage;
    [SerializeField] private Image backCircleImage;
    [SerializeField] private TextMeshProUGUI timerText;

    private float _maxTime;
    private float _remainingTime;
    private Action _onComplete;
    private bool _isRunning;
    private int _lastDisplayedSecond;
    private bool _isShaking;

    private Tween _pulseTween;
    private Tween _circlePulseTween;
    private Tween _shakeTween;

    public void StartTimer(float duration, Action onCompleteCallback)
    {
        _remainingTime = duration;
        _maxTime = duration;
        _onComplete = onCompleteCallback;
        _isRunning = true;
        _isShaking = false;
        _lastDisplayedSecond = Mathf.CeilToInt(_remainingTime);
        timerFillImage.fillAmount = 1f;
        UpdateText();
        StopAllAnimations();
    }

    public void StopTimer()
    {
        _isRunning = false;
        _onComplete = null;
        StopAllAnimations();
    }

    private void Update()
    {
        if (!_isRunning) return;

        _remainingTime -= Time.deltaTime;
        if (_remainingTime <= 0f)
        {
            _remainingTime = 0f;
            _isRunning = false;
            timerFillImage.fillAmount = 0f;
            UpdateText();
            StopAllAnimations();
            _onComplete?.Invoke();
        }
        else
        {
            timerFillImage.fillAmount = _remainingTime / _maxTime;
            int currentSecond = Mathf.CeilToInt(_remainingTime);

            if (currentSecond != _lastDisplayedSecond)
            {
                _lastDisplayedSecond = currentSecond;
                UpdateText();
                PulseAnimation();
            }

            if (currentSecond <= 5 && !_isShaking)
            {
                _isShaking = true;
                ShakeAnimation();
            }
        }
    }

    private void UpdateText()
    {
        timerText.text = Mathf.CeilToInt(_remainingTime).ToString();
    }

    private void PulseAnimation()
    {
        if(_pulseTween != null) _pulseTween.Kill();
        if(_circlePulseTween != null) _circlePulseTween.Kill();
        
        _pulseTween = transform.DOScale(new Vector3(0.1f,0.1f,0.1f), 0.1f).SetEase(Ease.OutQuad).SetRelative(true).SetLoops(2, LoopType.Yoyo);
        _circlePulseTween = backCircleImage.transform.DOScale(new Vector3(0.1f,0.1f,0.1f), 0.1f).SetEase(Ease.OutQuad).SetRelative(true).SetLoops(2, LoopType.Yoyo);
    }

    private void ShakeAnimation()
    {
        if (_shakeTween != null && _shakeTween.IsActive()) return;
        
        _shakeTween = transform.DORotate(new Vector3(0, 0, 20f), 0.1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopAllAnimations()
    {
        _isShaking = false;
        
        if (_pulseTween != null) _pulseTween.Kill();
        if (_circlePulseTween != null) _circlePulseTween.Kill();
        if (_shakeTween != null) _shakeTween.Kill();
        _pulseTween = null;
        _circlePulseTween = null;
        _shakeTween = null;
        
        transform.localScale = Vector3.one;
        backCircleImage.transform.localScale = Vector3.one;
    }
}
