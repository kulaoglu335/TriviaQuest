using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionNumberText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private int currentScore;
    private Tween scoreTween;

    public void SetQuestionNumber(int current, int total)
    {
        questionNumberText.text = "QUESTION "+$"{current}/{total}";
    }

    public void UpdateScore(int newScore)
    {
        if (scoreTween != null && scoreTween.IsActive()) scoreTween.Kill();

        int startScore = currentScore;
        scoreTween = DOTween.To(() => startScore, x =>
        {
            scoreText.text = x.ToString();
        }, newScore, 0.5f).OnComplete(() =>
        {
            currentScore = newScore;
        });
    }
}
