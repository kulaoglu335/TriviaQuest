using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AnswerButton : MonoBehaviour
{
    private int answerIndex = 0;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    public void SetButtonFuncs(int index, Action<int> onClickCallback)
    {
        answerIndex = index;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(answerIndex));
    }

    public void SetAnswer(string answerText)
    {
        this.answerText.text = answerText;
    }
}
