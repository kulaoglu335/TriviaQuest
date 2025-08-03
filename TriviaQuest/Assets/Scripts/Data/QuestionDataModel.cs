using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionData
{
    public string category;
    public string question;
    public string[] choices;
    public string answer;
}

[System.Serializable]
public class QuestionListWrapper
{
    public QuestionData[] questions;
}
