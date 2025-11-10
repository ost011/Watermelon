using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    private static LoadingManager instance = null;
    public static LoadingManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<LoadingManager>();
            }

            return instance;
        }
    }

    public GameObject loadingPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowLoadingPanel()
    {
        this.loadingPanel.SetActive(true);
    }

    public void HideLoadingPanel()
    {
        this.loadingPanel.SetActive(false);
    }
}
