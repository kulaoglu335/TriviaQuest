using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_LeaderboardSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void SetData(LeaderboardPlayer player)
    {
        _rankText.text = player.rank.ToString();
        _nicknameText.text = player.nickname;
        _scoreText.text = player.score.ToString();
    }
}
