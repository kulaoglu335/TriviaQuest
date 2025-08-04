using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class M_Quest : MonoBehaviour
{
    private QuestionData[] _questions;
    public QuestUIHandler uiHandler;
    
    private int _currentIndex;
    private bool _timerActive;
    private float _questionDuration;

    [Space(10)]
    [SerializeField] private float buttonOpenEffectDuration;
    [SerializeField] private float buttonCloseEffectDuration;
    [SerializeField] private float buttonEffectDelay;
    private Tween _answerEffectScaleTween;
    private Tween _answerEffectFadeTween;

    private int _correctScore;
    private int _incorrectScore;
    private int _timeOutScore;
    private int _totalScore;

    private void Awake()
    {
        M_Game.I.RegisterQuest(this);
    }
    public void Init(int correctScore, int incorrectScore, int timeOutScore, float questionDuration)
    {
        _correctScore = correctScore;
        _incorrectScore = incorrectScore;
        _timeOutScore = timeOutScore;
        _questionDuration = questionDuration;
        
        for (int i = 0; i < uiHandler.answerButtons.Count; i++)
        {
            uiHandler.answerButtons[i].InitButton(
                i, 
                OnClickAnswer,
                buttonOpenEffectDuration,
                buttonCloseEffectDuration,
                buttonEffectDelay);
        }
    }

    public void StartQuest(string questionURL,float delay)
    {
        _currentIndex = 0;
        _totalScore = 0;
        _timerActive = true;
        StartCoroutine(LoadQuestions(questionURL,delay));
    }
    public void FinishQuest()
    {
        
    }

    private void OnClickAnswer(int buttonIndex)
    {
        if(!_timerActive) return;
        _timerActive = false;
        
        uiHandler.StopTimer();
        
        char selectedChar = (char)('A' + buttonIndex);
        string selectedAnswer = selectedChar.ToString();
        string correctAnswer = _questions[_currentIndex].answer;
        
        if (selectedAnswer == correctAnswer)
        {
            _totalScore += _correctScore;
            uiHandler.HighlightAnswer(buttonIndex,true);
            uiHandler.CorrectEffect(_totalScore);
        }
        else
        {
            _totalScore += _incorrectScore;
            int correctIndex = _questions[_currentIndex].answer[0] - 'A';
            uiHandler.HighlightAnswer(buttonIndex,false);
            uiHandler.HighlightAnswer(correctIndex,true);
            uiHandler.IncorrectEffect(_totalScore);
        }

        StartCoroutine(SetReadyToNextQuestion());
    }
    private void TimeOut()
    {
        _timerActive = false;
        
        int correctIndex = _questions[_currentIndex].answer[0] - 'A';
        uiHandler.HighlightAnswer(correctIndex,true);
        _totalScore += _timeOutScore;
        uiHandler.IncorrectEffect(_totalScore);
        StartCoroutine(SetReadyToNextQuestion());
    }
    private IEnumerator SetReadyToNextQuestion()
    {
        yield return new WaitForSeconds(1f);
        
        uiHandler.CloseButtonUIs();

        float buttonEffectsWaitTime = buttonCloseEffectDuration + (3 * buttonEffectDelay) + 0.5f;
        yield return new WaitForSeconds(buttonEffectsWaitTime);
        
        _currentIndex++;
        uiHandler.SetQuestionData(_questions[_currentIndex],TimeOut,_currentIndex,_questions.Length,_questionDuration);
        _timerActive = true;
    }
    
    private IEnumerator LoadQuestions(string url,float delay)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string rawJson = req.downloadHandler.text;
            ParseBrokenJson(rawJson);
        }
        
        yield return new WaitForSeconds(delay);
        
        uiHandler.SetQuestionData(_questions[_currentIndex],TimeOut,_currentIndex,_questions.Length,_questionDuration);
    }
    private void ParseBrokenJson(string rawJson)
    {
        int start = rawJson.IndexOf("[{");
        int end = rawJson.LastIndexOf("}]");

        if (start == -1 || end == -1)
        {
            Debug.LogError("JSON formatı tanınamadı.");
            return;
        }

        string listContent = rawJson.Substring(start, end - start + 2);

        // Normalize: Tüm aralar tek satıra gelsin
        listContent = Regex.Replace(listContent, @"\r\n?|\n", " "); // Satır sonlarını temizle
        listContent = Regex.Replace(listContent, @"\s+", " ");     // Gereksiz boşlukları sil

        List<QuestionData> parsedQuestions = new List<QuestionData>();

        // Split için daha sağlam tanım
        string[] rawQuestions = Regex.Split(listContent, @"(?<=\}),(?=\s*\{)");

        foreach (string qRaw in rawQuestions)
        {
            QuestionData qData = ParseSingleQuestion(qRaw);
            if (qData != null)
                parsedQuestions.Add(qData);
        }

        _questions = parsedQuestions.ToArray();
    }
    private QuestionData ParseSingleQuestion(string raw)
    {
        try
        {
            string clean = raw.Trim().TrimStart('{').TrimEnd('}');

            string category = Regex.Match(clean, @"""category""\s*:\s*""([^""]*)""").Groups[1].Value;
            string question = Regex.Match(clean, @"""question""\s*:\s*""([^""]*)""").Groups[1].Value;
            string answer = Regex.Match(clean, @"""answer""\s*:\s*""([^""]*)""").Groups[1].Value;

            Match choiceMatch = Regex.Match(clean, @"""choices""\s*:\s*\[(.*?)\]");
            if (!choiceMatch.Success)
            {
                Debug.LogError("Choices parse failed");
                return null;
            }

            string rawChoices = choiceMatch.Groups[1].Value;

            // Tüm itemleri çek: Tırnak içi olanları tek tek al
            var choices = Regex.Matches(rawChoices, @"""(.*?)""")
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();

            if (choices.Length != 4)
                Debug.LogWarning($"Choice count issue: {choices.Length} - Soru: {question}");

            return new QuestionData
            {
                category = category,
                question = question,
                answer = answer,
                choices = choices
            };
        }
        catch (Exception e)
        {
            Debug.LogError("ParseSingleQuestion Error: " + e.Message);
            return null;
        }
    }
}
