using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UI_LeaderboardSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private Tween _moveTween;

    public void SetData(LeaderboardPlayer player)
    {
        rankText.text = player.rank.ToString();
        nicknameText.text = player.nickname;
        scoreText.text = player.score.ToString();
    }

    /// <summary>
    /// Animasyon tipi:
    /// 1 = Center to Left
    /// 2 = Right to Center
    /// 3 = Center to Right
    /// 4 = Left to Center
    /// </summary>
    
    public IEnumerator ApplyAnim(float duration, float delay, int firstAnimType, int secondAnimType, float horizontalDistance, LeaderboardPlayer player)
    {
        gameObject.SetActive(true);

        if (firstAnimType != 0)
        {
            yield return Animate(firstAnimType, duration, delay, horizontalDistance, Ease.InCubic);
        }

        SetData(player);

        if (secondAnimType != 0)
        {
            yield return Animate(secondAnimType, duration, 0f, horizontalDistance, Ease.OutCubic);
        }
    }

    private IEnumerator Animate(int animType, float duration, float delay, float horizontalDistance, Ease ease)
    {
        Vector3 startPos = GetAnimStartPos(animType, horizontalDistance);
        Vector3 endPos = GetAnimEndPos(animType, horizontalDistance);

        startPos.y = transform.localPosition.y;
        endPos.y = transform.localPosition.y;

        if (_moveTween != null) _moveTween.Kill();

        transform.localPosition = startPos;
        _moveTween = transform.DOLocalMove(endPos, duration).SetEase(ease).SetDelay(delay);

        yield return new WaitForSeconds(duration + delay);
    }

    private Vector3 GetAnimStartPos(int animType, float horizontalDistance)
    {
        return animType switch
        {
            2 => new Vector3(horizontalDistance, 0, 0),
            4 => new Vector3(-horizontalDistance, 0, 0),
            _ => Vector3.zero // for 1 and 3
        };
    }

    private Vector3 GetAnimEndPos(int animType, float horizontalDistance)
    {
        return animType switch
        {
            1 => new Vector3(-horizontalDistance, 0, 0),
            3 => new Vector3(horizontalDistance, 0, 0),
            _ => Vector3.zero // for 2 and 4
        };
    }
}
