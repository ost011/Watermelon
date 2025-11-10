using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CurrentRankViewType
{
    MyScore,
    WorldRank,
}

public class RankingPopUp : AbstractPopUp
{
    public GameObject[] worldRankingElements;
    public GameObject[] myRankingElements;

    [Space]
    [Header("랭킹 세부 리스트들 ---------------")]
    public RankingListModule[] arrayMyRankListModules;
    public WorldRankingListModule[] arrayWorldRankListModules;
    public MyRankInWorldRankingModule myRankListModuleInWorldRanking;
    
    
    [SerializeField]
    private CurrentRankViewType currentRankViewType = CurrentRankViewType.MyScore;
    public CurrentRankViewType CurrentRankViewType => this.currentRankViewType;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void ShowPopUp()
    {
        if (this.currentRankViewType.Equals(CurrentRankViewType.MyScore))
        {
            // 처음 팝업을 열었거나 마지막으로 보던 창이 내 최고 점수이다
            ShowMyBestScoreView();
        }
        else
        {
            // 마지막으로 보던 창이 월드 랭킹이다
            ShowWorldRankingView();
        }

        base.ShowPopUp();
    }

    public void OnClickClosePopUpBtn()
    {
        HidePopUp();
    }

    public void OnClickMyRankingBtn()
    {
        CustomDebug.Log(">>>> OnClickMyRankingBtn <<<<");

        if (this.currentRankViewType.Equals(CurrentRankViewType.MyScore))
        {
            CustomDebug.Log("you are already watching MyScore View !");
            return;
        }

        ShowMyBestScoreView();
    }

    public void OnClickWorldRankingBtn()
    {
        CustomDebug.Log(">>>> OnClickWorldRankingBtn <<<<");

        if (this.currentRankViewType.Equals(CurrentRankViewType.WorldRank))
        {
            CustomDebug.Log("you are already watching WorldRanking View !");
            return;
        }

        ShowWorldRankingView();
    }

    private void ShowMyBestScoreView()
    {
        HideAllRankingListModules();

        this.currentRankViewType = CurrentRankViewType.MyScore;

        LoadingManager.Instance.ShowLoadingPanel();

        var userScoreListPathStr = DevUtil.Instance.GetDBTargetPathString(EnumSets.DBParentType.Users, UserManager.Instance.GetUserId());

        FirebaseDatabaseManager.Instance.GetValueAsync(userScoreListPathStr, (dataSnapShot) =>
        {
            var myDataTable = dataSnapShot.Value as Dictionary<string, object>;
            var scoreListAsObject = myDataTable[Constants.DATABASE_SCORE_LIST_PATH_STR] as List<object>;

            var scoreList = DevUtil.Instance.GetListOfInt(scoreListAsObject);

            for (int i = 0; i < scoreList.Count; i++)
            {
                var score = scoreList[i];

                if (score == 0)
                {
                    // 기본적으로 생성시 리스트에 0 값이 하나 들어감. 0 도 표시가 필요하면 해당 구문 제거하기
                    continue;
                }

                var currentMyRankListModule = this.arrayMyRankListModules[i];

                currentMyRankListModule.gameObject.SetActive(true);

                currentMyRankListModule.UpdateScore(score);
            }

            ShowMyRankElements();
            HideWorldRankElements();

            LoadingManager.Instance.HideLoadingPanel();
        });
    }

    private void ShowWorldRankingView()
    {
        HideAllRankingListModules();

        this.currentRankViewType = CurrentRankViewType.WorldRank;

        LoadingManager.Instance.ShowLoadingPanel();

        FirebaseDatabaseManager.Instance.LoadRankingDataAsync((top5RankingTable, myRankStr) =>
        {
            if (top5RankingTable == null || myRankStr == null)
            {
                return;
            }

            // 상위 5위 랭킹 데이터를 가져왔고, 내 등수(string) 도 받아왔다
            // top5RankingTable 에는 displayName (닉네임), score 가 포함됨

            var rank = 0;

            foreach (var top5RankElement in top5RankingTable)
            {
                var currentWorldRankingListModule = this.arrayWorldRankListModules[rank];

                currentWorldRankingListModule.gameObject.SetActive(true); // rank 번째 리스트 켜기
                rank++;

                var userId = top5RankElement.Key;
                var specificUserDataTable = top5RankElement.Value;

                var displayName = specificUserDataTable[Constants.DATABASE_DISPLAY_NAME_PATH_STR] as string;
                var score = Convert.ToInt32(specificUserDataTable[Constants.DATABASE_SCORE_PATH_STR]);

                currentWorldRankingListModule.UpdateDisplayName(displayName);
                currentWorldRankingListModule.UpdateScore(score);
                CustomDebug.Log($"rank : {rank}, displayName : {displayName}, score : {score}");
            }

            FirebaseDatabaseManager.Instance.LoadSpecificUserBestScoreAsync(UserManager.Instance.GetUserId(), (myBestScore) =>
            {
                this.myRankListModuleInWorldRanking.UpdateMyRankText(myRankStr);
                this.myRankListModuleInWorldRanking.UpdateDisplayName(UserManager.Instance.GetDisplayName());
                this.myRankListModuleInWorldRanking.UpdateScore(myBestScore);

                ShowWorldRankElements();
                HideMyRankElements();

                LoadingManager.Instance.HideLoadingPanel();
            });
        });
    }

    public void ShowMyRankElements()
    {
        foreach(var obj in myRankingElements)
        {
            obj.SetActive(true);
        }
    }

    public void HideMyRankElements()
    {
        foreach (var obj in myRankingElements)
        {
            obj.SetActive(false);
        }
    }

    public void ShowWorldRankElements()
    {
        foreach (var obj in worldRankingElements)
        {
            obj.SetActive(true);
        }
    }

    public void HideWorldRankElements()
    {
        foreach (var obj in worldRankingElements)
        {
            obj.SetActive(false);
        }
    }

    public void HideAllRankingListModules()
    {
        for (int i = 0; i < 5; i++) 
        {
            this.arrayMyRankListModules[i].gameObject.SetActive(false);
            this.arrayWorldRankListModules[i].gameObject.SetActive(false);
        }
    }
}
