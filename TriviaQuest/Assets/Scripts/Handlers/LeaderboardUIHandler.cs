using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardUIHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lbPageText;
    
    public void UpdatePageText(int currentPage)
    {
        if (lbPageText != null)
            lbPageText.text = (currentPage + 1).ToString();
    }
}
