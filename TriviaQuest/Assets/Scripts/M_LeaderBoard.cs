using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class M_LeaderBoard : MonoBehaviour
{
    public static M_LeaderBoard I { get; private set; }

    private LeaderboardData currentData;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    public IEnumerator FetchLeaderboard(int page = 0)
    {
        #if UNITY_EDITOR
            string url = $"https://magegamessite.web.app/case1/leaderboard_page_{page}.json";
        #else
            string url = $"http://localhost:8080/leaderboard?page={page}";
        #endif

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                currentData = JsonUtility.FromJson<LeaderboardData>(request.downloadHandler.text);
                Debug.Log($"Leaderboard page {page} fetched successfully!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("JSON Parse Error: " + ex.Message);
                currentData = null;
            }
        }
        else
        {
            Debug.LogError("Leaderboard fetch failed: " + request.error);
            currentData = null;
        }
    }

    public LeaderboardData GetLatestData()
    {
        return currentData;
    }
}
