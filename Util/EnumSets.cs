using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumSets
{
    public enum DBParentType
    {
        Users,
        Rank,
    }

    public enum GameConceptType
    {
        Fruit,
        Coin,
    }
    
    public enum CircleLevel
    {
        Level_1,
        Level_2,
        Level_3,
        Level_4,
        Level_5,
        Level_6,
        Level_7,
        Level_8,
        Level_9,
        Level_10,
        Level_11,
        None
    }

    public enum InGameSoundClipType
    {
        ReleaseCircle,
        CircleCollided,
        CircleMerged,
    }

    public enum CircleImageTypeBySituation
    {
        Appear,
        WhileDragAndDrop,
        Collided,
        Default,
    }

    public enum PopUpType
    {
        Setting,
        Ranking,
        GameResult,
    }

    public enum LanguageType
    {
        English,
        Korean,
    }
}
