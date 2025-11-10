using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppInfo
{
    public static AppInfo Instance = null;

    static AppInfo()
    {
        Instance = new AppInfo();
    }

    private AppInfo()
    {
        
    }

    //private EnumSets.GameConceptType gameConceptType = EnumSets.GameConceptType.Fruit;
    //public EnumSets.GameConceptType GameConceptType => this.gameConceptType;

    public void Init()
    {
        //CheckGameConceptVersion();
    }
    

    private void CheckGameConceptVersion()
    {
//#if COIN_VER && !FRUIT_VER
//        this.gameConceptType = EnumSets.GameConceptType.Coin;
//#elif FRUIT_VER && !COIN_VER
//        this.gameConceptType = EnumSets.GameConceptType.Fruit;
//#endif

//        CustomDebug.Log($"You are running {gameConceptType} Version~~");
    }
}
