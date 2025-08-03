using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class M_Question : MonoBehaviour
{
    public static M_Question I { get; private set; }

    private QuestionData[] _questions;
    private int _currentIndex;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    public IEnumerator LoadQuestions(string url)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            _questions = JsonUtility.FromJson<QuestionListWrapper>(req.downloadHandler.text).questions;
            _currentIndex = 0;
        }
        else
        {
            Debug.LogError("Question fetch failed: " + req.error);
        }
    }

    public QuestionData GetCurrentQuestion() => _questions[_currentIndex];
    public bool HasNextQuestion() => _currentIndex + 1 < _questions.Length;
    public void NextQuestion() => _currentIndex++;
}
