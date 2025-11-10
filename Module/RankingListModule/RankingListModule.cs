using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingListModule : MonoBehaviour
{
    public TextMeshProUGUI textScore;
    
    public void UpdateScore(int value)
    {
        this.textScore.text = value.ToString();
    }
}
