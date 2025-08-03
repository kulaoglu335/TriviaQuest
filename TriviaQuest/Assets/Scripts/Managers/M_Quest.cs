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
    private int _currentIndex;

    [Header("Quest UI")]
    [SerializeField] private List<UI_AnswerButton> answerButtons;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private UI_Category categoryUI;
    [SerializeField] private UI_Timer timerUI;
    [SerializeField] private UI_Goal goalUI;
    [SerializeField] private Color32 normalAnswerColor;
    [SerializeField] private Color32 correctAnswerColor;
    [SerializeField] private Color32 incorrectAnswerColor;
    
    [Space(10)]
    [SerializeField] private List<string> correctAnswerStrings;
    [SerializeField] private List<string> incorrectAnswerStrings;
    [SerializeField] private TextMeshProUGUI correctAnswerEffectText;
    [SerializeField] private TextMeshProUGUI incorrectAnswerEffectText;
    private Tween _answerEffectScaleTween;
    private Tween _answerEffectFadeTween;

    [Space(10)]
    [SerializeField] private float buttonOpenEffectDuration;
    [SerializeField] private float buttonCloseEffectDuration;
    [SerializeField] private float buttonEffectDelay;

    private void Awake()
    {
        M_Game.I.RegisterQuest(this);
    }
    public void Init()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].SetButtonFuncs(i, OnClickAnswer,normalAnswerColor, correctAnswerColor, incorrectAnswerColor,buttonOpenEffectDuration,buttonCloseEffectDuration,buttonEffectDelay);
        }
    }

    public void StartQuest(string questionURL)
    {
        StartCoroutine(LoadQuestions(questionURL));
    }
    public void FinishQuest()
    {
        
    }

    private void SetReadyToQuestion(int index)
    {
        questionText.text = _questions[index].question;
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].SetReady(_questions[index].choices[i]);
        }
        
        correctAnswerEffectText.gameObject.SetActive(false);
        incorrectAnswerEffectText.gameObject.SetActive(false);
    }
    private void OnClickAnswer(int buttonIndex)
    {
        char selectedChar = (char)('A' + buttonIndex);
        string selectedAnswer = selectedChar.ToString();
        string correctAnswer = _questions[_currentIndex].answer;
        
        if (selectedAnswer == correctAnswer)
        {
            answerButtons[buttonIndex].CorrectAnswerEffect();
            CorrectEffect();
        }
        else
        {
            int correctIndex = _questions[_currentIndex].answer[0] - 'A';
            answerButtons[buttonIndex].IncorrectAnswerEffect();
            answerButtons[correctIndex].CorrectAnswerEffect();
            IncorrectEffect();
        }

        StartCoroutine(SetReadyToNextQuestion());
        
    }
    private void CorrectEffect()
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
    }
    private void IncorrectEffect()
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
    }
    private IEnumerator SetReadyToNextQuestion()
    {
        yield return new WaitForSeconds(1f);
        
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].CloseEffect();
        }

        float buttonEffectsWaitTime = buttonCloseEffectDuration + ((answerButtons.Count - 1) * buttonEffectDelay) + 0.5f;
        yield return new WaitForSeconds(buttonEffectsWaitTime);
        
        _currentIndex++;
        SetReadyToQuestion(_currentIndex);
    }
    
    private IEnumerator LoadQuestions(string url)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string rawJson = req.downloadHandler.text;
            ParseBrokenJson(rawJson);
            
            _currentIndex = 0;
        }
        else
        {
            Debug.LogError("Question fetch failed: " + req.error);
        }
        
        SetReadyToQuestion(_currentIndex);
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
        _currentIndex = 0;
        SetReadyToQuestion(_currentIndex);
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
