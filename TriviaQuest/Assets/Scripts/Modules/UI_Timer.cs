using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UI_Timer : MonoBehaviour
{
    [SerializeField] private Image timerFillImage;
    [SerializeField] private TextMeshProUGUI timerText;

    private float _maxTime;
    private float _remainingTime;
    private Action _onComplete;
    private bool _isRunning;

    public void StartTimer(float duration, Action onCompleteCallback)
    {
        _remainingTime = duration;
        _maxTime = duration;
        _onComplete = onCompleteCallback;
        _isRunning = true;
        timerFillImage.fillAmount = 1f;
        UpdateText();
    }

    public void StopTimer()
    {
        _isRunning = false;
        _onComplete = null;
    }

    private void Update()
    {
        if (!_isRunning) return;

        _remainingTime -= Time.deltaTime;
        if (_remainingTime <= 0f)
        {
            _remainingTime = 0f;
            _isRunning = false;
            UpdateText();
            timerFillImage.fillAmount = 0f;
            _onComplete?.Invoke();
        }
        else
        {
            UpdateText();
            timerFillImage.fillAmount = _remainingTime / _maxTime;
        }
    }

    private void UpdateText()
    {
        timerText.text = Mathf.CeilToInt(_remainingTime).ToString();
    }
}
