using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderboardData
{
    public int page;
    public bool is_last;
    public LeaderboardPlayer[] data;
}

[System.Serializable]
public class LeaderboardPlayer
{
    public int rank;
    public string nickname;
    public int score;
}
