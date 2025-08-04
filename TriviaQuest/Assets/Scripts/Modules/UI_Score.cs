using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionNumberText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Color32 normalColor;
    [SerializeField] private Color32 correctColor;
    [SerializeField] private Color32 incorrectColor;
    
    private int _currentScore;
    private Tween _scoreChangeTween;
    private Tween _scoreColorTween;
    private Tween _scoreScaleTween;

    public void ResetScore()
    {
        _currentScore = 0;
        scoreText.text = _currentScore.ToString();
        scoreText.color = normalColor;
        scoreText.transform.localScale = Vector3.one;
    }

    public void SetQuestionNumber(int current, int total)
    {
        questionNumberText.text = "QUESTION "+$"{current}/{total}";
    }

    public void UpdateScore(int newScore,bool isCorrect)
    {
        scoreText.color = normalColor;
        scoreText.transform.localScale = Vector3.one;
        HighlightText(isCorrect);
        
        if (_scoreChangeTween != null && _scoreChangeTween.IsActive()) _scoreChangeTween.Kill();

        int startScore = _currentScore;
        _scoreChangeTween = DOTween.To(() => startScore, x =>
        {
            scoreText.text = x.ToString();
        }, newScore, 0.5f).OnComplete(() =>
        {
            _currentScore = newScore;
            _scoreColorTween = scoreText.DOColor(normalColor, 0.5f).SetDelay(0.2f);
            _scoreScaleTween = scoreText.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.2f);
        });
    }

    private void HighlightText(bool isCorrect)
    {
        if(_scoreColorTween != null) _scoreColorTween.Kill();
        Color32 newColor = isCorrect ? correctColor : incorrectColor;
        _scoreColorTween = scoreText.DOColor(newColor, 0.1f);
        
        if(_scoreScaleTween != null) _scoreScaleTween.Kill();
        _scoreScaleTween = scoreText.transform.DOScale(new Vector3(1.4f,1.4f,1.4f), 0.25f);
    }
}
