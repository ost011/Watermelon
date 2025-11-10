using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyRankInWorldRankingModule : RankingListModule
{
    public TextMeshProUGUI textDisplayName;
    public TextMeshProUGUI textRank;

    public void UpdateDisplayName(string value)
    {
        this.textDisplayName.text = value;
    }

    public void UpdateMyRankText(string value)
    {
        this.textRank.text = value;
    }
}
