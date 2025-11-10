using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastPopUpManager : MonoBehaviour
{
    private static ToastPopUpManager instance = null;
    public static ToastPopUpManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<ToastPopUpManager>();
            }

            return instance;
        }
    }

    public RectTransform transformToastPopUp;
    public TextMeshProUGUI textToastMessage;

    private IEnumerator toastMessageEffectEnumerator = null;
    private WaitForSeconds toastMessageDuration = null;

    private Vector2 toastPopUpOriginalPos = new Vector2(0, 50);
    private Vector2 toastPopUpShownPos = new Vector2(0, -120);

    private const float TOAST_MESSAGE_DURATION = 0.6f;
    
    // Start is called before the first frame update
    void Start()
    {
        this.toastMessageDuration = new WaitForSeconds(TOAST_MESSAGE_DURATION);
    }

    public void ShowToastPopUpMessage(string message)
    {
        ShowToastMessageEffect(message);
    }

    private void ShowToastMessageEffect(string message)
    {
        if(toastMessageEffectEnumerator == null)
        {
            toastMessageEffectEnumerator = CorShowToastMessageEffect(message);
        }
        else
        {
            return;
        }

        StartCoroutine(toastMessageEffectEnumerator);
    }

    private IEnumerator CorShowToastMessageEffect(string message)
    {
        this.textToastMessage.text = message;
        this.transformToastPopUp.gameObject.SetActive(true);

        this.transformToastPopUp.DOLocalMove(this.toastPopUpShownPos, 0.3f).SetEase(Ease.OutCubic).OnComplete(() => 
        {
            ShakeToastPopUp(ResetToastPopUp);
        });

        yield return toastMessageDuration;

        toastMessageEffectEnumerator = null;
    }

    private void ShakeToastPopUp(Action onComplete = null)
    {
        this.transformToastPopUp.DOShakePosition(0.3f, 2f, 15).OnComplete(() => 
        {
            onComplete?.Invoke();
        });
    }

    private void MoveToastPopUpToOriginalPos()
    {
        // this.transformToastPopUp.DOLocalMoveY(100f, 0f);
        this.transformToastPopUp.localPosition = toastPopUpOriginalPos;
    }

    private void ResetToastPopUp()
    {
        MoveToastPopUpToOriginalPos();
        transformToastPopUp.gameObject.SetActive(false);
    }
}
