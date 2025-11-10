using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameSceneUiManager : MonoBehaviour
{
    //public GameObject fruitBaseUis;
    //public GameObject coinBaseUis;

    [Space]
    [Header("Upper Uis -----------------------")]
    public Image nextCircleImage; // 우측 상단 다음 오브젝트 이미지

    public TextMeshProUGUI textScore; // 최상단 현재 점수
    public TextMeshProUGUI textBestScore; // 최상단 현재 점수

    public GameObject objReachedFinalLevel; // 좌측 상단 통장 도달 카운트 요소들
    public RectTransform rtReachedFinalLevel; // 최종 레벨인 통장 도달 시 모션에 필요함
    public TextMeshProUGUI textReachedFinalCircle;// 좌측 상단 통장 도달 카운트

    [Space]
    [Header("RetryElements ------------------------")]
    // public GameObject popupRetry;
    public TextMeshProUGUI textFinalScoreInRetryPopUp;
    public TextMeshProUGUI textHighestScoreInRetryPopUp;
    public TextMeshProUGUI textHighestScoreInRankingPopUp;

    //[Space]
    //[Header("PopUps -------------------------")]
    //public GameObject popupSetting;
    //public GameObject popupRanking;

    //[Space]
    //[Header("BGs")]
    //public GameObject popupBlackBG;


    //public void ShowFruitBaseUis()
    //{
    //    this.fruitBaseUis.SetActive(true);
    //}

    //public void HideFruitBaseUis()
    //{
    //    this.fruitBaseUis.SetActive(false);
    //}

    //public void ShowCoinBaseUis()
    //{
    //    this.coinBaseUis.SetActive(true);
    //}

    //public void HideCoinBaseUis()
    //{
    //    this.coinBaseUis.SetActive(false);
    //}

    public void UpdateNextCircleImage(Sprite sprite)
    {
        this.nextCircleImage.sprite = sprite;
    }

    public void UpdateScoreText(int score)
    {
        this.textScore.text = score.ToString();
    }

    public void UpdateBestScoreText(int score)
    {
        this.textBestScore.text = score.ToString();
    }

    public void UpdateReachedFinalCircleCount(int count)
    {
        this.textReachedFinalCircle.text = count.ToString();
    }

    public void ShowReachedFinalLevelUis()
    {
        objReachedFinalLevel.SetActive(true);
    }

    public void MoveFinalLevelCoinToUpperLeftUi(RectTransform target, Action onComplete = null)
    {
        // target.DOScale(1.2f, 0.1f);
        // target.DOScale(0.7f, 2f);
        target.DOScale(0.3f, 1f).SetDelay(0.5f);
        target.DOMove(this.rtReachedFinalLevel.position, 1.2f).SetEase(Ease.InQuad).SetDelay(0.2f).OnComplete(() =>
        {
            onComplete?.Invoke();
            ShowFinalCoinScaleAnimation();
        });

        //target.DOScale(0.3f, 0.5f).OnComplete(() => 
        //{
        //    onComplete?.Invoke();
        //});

        //target.DOScale(1.2f, 0.2f);
        //target.DOScale(0.7f, 0.3f);
        //rtReachedFinalLevel.DOScale(1.3f, 0.3f).OnComplete(() => 
        //{
        //    rtReachedFinalLevel.DOScale(1f, 0.3f).OnComplete(() => 
        //    {
        //        onComplete?.Invoke();
        //    });

        //});
    }

    public void ShowFinalCoinScaleAnimation()
    {
        this.rtReachedFinalLevel.DOKill();

        rtReachedFinalLevel.DOScale(1.3f, 0.2f).OnComplete(() =>
        {
            rtReachedFinalLevel.DOScale(1f, 0.2f);
        });
    }

    //public void ShowRetryPopUp()
    //{
    //    this.popupRetry.SetActive(true);
    //    // ShowPopUpBlackBG();
    //}

    //public void HideRetryPopUp()
    //{
    //    this.popupRetry.SetActive(false);
    //    // HidePopUpBlackBG();
    //}

    public void UpdateFinalScoreInRetryPopUp(int score)
    {
        this.textFinalScoreInRetryPopUp.text = score.ToString();
    }

    public void UpdateHighestScoreInRetryPopUp(int score)
    {
        this.textHighestScoreInRetryPopUp.text = score.ToString();
    }

    public void UpdateHighestScoreInRankingPopUp(int score)
    {
        this.textHighestScoreInRankingPopUp.text = score.ToString();
    }

    //public void ShowSettingPopUp()
    //{
    //    this.popupSetting.SetActive(true);
    //}

    //public void HideSettingPopUp()
    //{
    //    this.popupSetting.SetActive(false);
    //}

    //public void ShowRankingPopUp()
    //{
    //    this.popupRanking.SetActive(true);
    //}

    //public void HideRankingPopUp()
    //{
    //    this.popupRanking.SetActive(false);
    //}

    //public void ShowPopUpBlackBG()
    //{
    //    this.popupBlackBG.SetActive(true);
    //}

    //public void HidePopUpBlackBG()
    //{
    //    this.popupBlackBG.SetActive(false);
    //}
}
