using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    private static LanguageManager instance = null;
    public static LanguageManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<LanguageManager>();
            }

            return instance;
        }
    }

    private Action<EnumSets.LanguageType> onLanguageChangedCallback = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChainCallbackOnLanguageChanged(Action<EnumSets.LanguageType> callback)
    {
        this.onLanguageChangedCallback += callback;
    }

    public void UnChainCallbackOnLanguageChanged(Action<EnumSets.LanguageType> callback)
    {
        this.onLanguageChangedCallback -= callback;
    }
}
