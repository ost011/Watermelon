using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameResultPopUp : AbstractPopUp
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnClickRetryBtn()
    {
        InGameSceneController.Instance.OnClickRetryBtn();
        
        HidePopUp();
    }
}
