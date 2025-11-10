using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldRankingListModule : RankingListModule
{
    public TextMeshProUGUI textDisplayName;

    public void UpdateDisplayName(string value)
    {
        this.textDisplayName.text = value;
    }
}
