using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class M_Quest : MonoBehaviour
{
    [SerializeField] private QuestionData[] _questions;
    private int _currentIndex;

    [Header("Quest UI")]
    [SerializeField] private List<UI_AnswerButton> answerButtons;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private UI_Category categoryUI;
    [SerializeField] private UI_Timer timerUI;
    [SerializeField] private UI_Goal goalUI;

    private void Awake()
    {
        M_Game.I.RegisterQuest(this);
    }

    public void Init()
    {
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].SetButtonFuncs(i, OnClickAnswer);
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
        //for (int i = 0; i < answerButtons.Count; i++)
        //{
        //    answerButtons[i].SetAnswer(_questions[index].choices[i]);
        //}
    }
    private void OnClickAnswer(int buttonIndex)
    {
        
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
        
        List<QuestionData> parsedQuestions = new List<QuestionData>();
        string[] rawQuestions = Regex.Split(listContent, @"},\s*{");

        foreach (string qRaw in rawQuestions)
        {
            Debug.LogError(qRaw);
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
            string clean = raw.Trim('{', '}');

            string category = Regex.Match(clean, @"""category""\s*:\s*""([^""]*)""").Groups[1].Value;
            string question = Regex.Match(clean, @"""question""\s*:\s*""([^""]*)""").Groups[1].Value;
            string answer = Regex.Match(clean, @"""answer""\s*:\s*""([^""]*)""").Groups[1].Value;

            string rawChoices = Regex.Match(clean, @"""choices""\s*:\s*\[(.*?)\]").Groups[1].Value;
            Debug.LogError(rawChoices);
            string[] choices = rawChoices.Split(new string[] { "\",\"" }, StringSplitOptions.None)
                .Select(s => s.Trim('"')).ToArray();

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
