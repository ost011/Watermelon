using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPopUp : MonoBehaviour
{
    public GameObject body;

    [SerializeField]
    private EnumSets.PopUpType popupType;
    public EnumSets.PopUpType PopupType => this.popupType;

    public virtual void ShowPopUp()
    {
        this.body.SetActive(true);
    }

    public virtual void HidePopUp()
    {
        this.body.SetActive(false);
    }
}
