using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPopUp : AbstractPopUp
{
    //public GameObject body;

    //public EnumSets.PopUpType popUpType;

    //public EnumSets.PopUpType GetPopUpType()
    //{
    //    return this.popUpType;
    //}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //public void ShowPopUp()
    //{
    //    body.SetActive(true);
    //}

    //public void HidePopUp()
    //{
    //    body.SetActive(false);
    //}

    // Close PopUp
    public void OnClickUpperRightExitBtn()
    {
        HidePopUp();
    }

    // Quit App, Later : Go To Lobby Scene
    public void OnClickExitBtn()
    {
        HidePopUp();

        Application.Quit();
    }

    public void OnClickRetryBtn()
    {
        HidePopUp();

        InGameSceneController.Instance.OnClickRetryBtn();
    }

    public void OnClickRemoveAdBtn()
    {
        CustomDebug.Log($"OnClickRemoveAdBtn");
        ToastPopUpManager.Instance.ShowToastPopUpMessage("In Preparation...");
    }
}
