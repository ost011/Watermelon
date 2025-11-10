using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneUiManager : MonoBehaviour
{
    public GameObject fadeoutBG;
    public Image imgPressAnyKey;

    [Space]
    [Header("Login Elements")]
    public GameObject objLoginElements;

    public void ShowPressAnyKeyImage()
    {
        this.imgPressAnyKey.gameObject.SetActive(true);
    }

    public void StartSceneConversionEffect()
    {
        imgPressAnyKey.GetComponent<DOTweenAnimation>().DOKill();

        imgPressAnyKey.transform.DOScale(1f, 0.2f);
        imgPressAnyKey.DOFillAmount(0f, 1.5f).SetDelay(1f).OnComplete(() =>
        {
            fadeoutBG.SetActive(true);
        });
    }

    public void ShowLoginElements()
    {
        this.objLoginElements.SetActive(true);
    }

    public void HideLoginElements()
    {
        this.objLoginElements.SetActive(false);
    }
}
