using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 게임 시작, 종료,
/// 전반적인 루틴을 담당하는
/// InGame 씬 컨트롤러
/// </summary>
public class InGameSceneController : MonoBehaviour
{
    private static InGameSceneController instance = null;
    public static InGameSceneController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<InGameSceneController>();
            }

            return instance;
        }
    }

    public InGameSceneUiManager inGameSceneUiManager;
    public CircleManager circleManager;
    public GameEndCheckModule gameEndCheckModule;

    public List<AbstractPopUp> wholePopUps = new List<AbstractPopUp>();

    private int reachedFinalCircleCount = 0;

    private int totalGamePlayCount = 0;

    private int currentScore = 0;
    
    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    #region Auto For Test
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        StartAutoPlay();
    //    }

    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        StopAutoPlay();
    //    }
    //}

    //private void StartAutoPlay()
    //{
    //    TouchScreenHandler.Instance.SetCurrentCircleDroppedCallback(RepeatSpawnNextCircleAfterDropped);

    //    TouchScreenHandler.Instance.StartAutoPlay();
    //}

    //private void StopAutoPlay()
    //{
    //    TouchScreenHandler.Instance.SetCurrentCircleDroppedCallback(CheckNextRoutineOnCurrentCircleDropped);

    //    TouchScreenHandler.Instance.StopAutoPlay();
    //}

    //private void RepeatSpawnNextCircleAfterDropped()
    //{
    //    TouchScreenHandler.Instance.SetUnDraggableState();
    //    TouchScreenHandler.Instance.HideDropCircleGuideImage();

    //    SpawnNextRandomCircleAfterDelay(0.5f);
    //}
    #endregion

    private void Init()
    {
        SetCallbacks();

        InitUiElements();

        InitManagers();
    }

    private void SetCallbacks()
    {
        TouchScreenHandler.Instance.SetCurrentCircleDroppedCallback(CheckNextRoutineOnCurrentCircleDropped);

        this.gameEndCheckModule.SetCheckGameEndCallback(CheckGameEnd);
    }

    private void InitUiElements()
    {
        UpdateMyBestScoreUIs(); // 내 최고 점수 업데이트

        UpdateScoreText(this.currentScore); // 현재 점수 0

        //this.inGameSceneUiManager.ShowFruitBaseUis();
        //this.inGameSceneUiManager.HideCoinBaseUis();
    }

    private void InitManagers()
    {
        this.circleManager.Init();

        GoogleAdsManager.Instance.Init();

        SoundManager.Instance.Init();
    }

    // 유저가 Circle 을 드래그하다 놓았을 때
    // 게임이 종료되는 지 다음 공을 소환할 지 판단하기
    // to do : 이제 여기서 체크하지 않으므로 이름을 바꾸던가 하기
    private void CheckNextRoutineOnCurrentCircleDropped()
    {
        TouchScreenHandler.Instance.SetUnDraggableState();
        TouchScreenHandler.Instance.HideDropCircleGuideImage();

        SpawnNextRandomCircleAfterDelay(0.5f);
    }

    private void CheckGameEnd(bool isGameEnd)
    {
        if (isGameEnd)
        {
            var target = from data in wholePopUps
                         where data.PopupType.Equals(EnumSets.PopUpType.GameResult)
                         select data;

            if(target.Count() > 0)
            {
                this.inGameSceneUiManager.UpdateFinalScoreInRetryPopUp(this.currentScore);

                ForceStopGameRoutine();

                SaveCurrentScore(() => 
                {
                    UpdateMyBestScoreUIs(); // 내 최고 점수 업데이트

                    target.First().ShowPopUp(); // RetryPopUp 띄우기

                    this.totalGamePlayCount++; // 게임 플레이 횟수 증가
                });
            }
            else
            {
                CustomDebug.Log("There is no GameResult Popup~!!!");
            }

            CustomDebug.LogWithColor("Game End !>!>!>!>!>!>!>!>!>!", CustomDebug.ColorSet.Red);
        }
        else
        {
            // 이미 다음 공은 생성되었고 돌아가고 있음
        }
    }

    private void SpawnNextRandomCircleAfterDelay(float delay)
    {
        StartCoroutine(CorSpawnNextCircle(delay));
    }

    private IEnumerator CorSpawnNextCircle(float delay)
    {
        yield return new WaitForSeconds(delay);

        SpawnNextRandomCircle();

        TouchScreenHandler.Instance.SetDraggableState();
        TouchScreenHandler.Instance.ShowDropCircleGuideImage();
    }

    // 게임 처음부터 실행하기
    // 재도전을 해야하거나 재도전 버튼을 눌렀을 때도 호출될 것
    // BlackBG 의 FadeOut-OnComplete 에 참조되어 있음
    public void InitGameRoutine()
    {
        UpdateScoreText(this.currentScore);

        SpawnNextRandomCircleAfterDelay(0f);
    }

    private void ForceStopGameRoutine()
    {
        // 현재 circle 을 드래그하지 못하게 하기

        TouchScreenHandler.Instance.SetUnDraggableState();
        TouchScreenHandler.Instance.HideDropCircleGuideImage();

        this.circleManager.ForceAllCirclesNotToMerge();
    }

    private void SaveCurrentScore(Action isReady)
    {
        var dataTable = new Dictionary<string, object>();

        #region users 데이터 업데이트
        var userPathStr = GetUserScoreListPathStr();

        var scoreList = UserManager.Instance.GetUserScoreList();

        if(scoreList == null)
        {
            scoreList = new List<int>();
        }

        scoreList.Add(this.currentScore);

        var tmpList = scoreList.OrderByDescending(x => x).ToList(); // 내림차순 정렬

        scoreList = tmpList;

        if (scoreList.Count > 5)
        {
            // Score 리스트는 5개까지만 표시함

            scoreList.RemoveAt(5);
        }
        #endregion

        #region rank 에 내 최고 점수 업데이트
        var rankPathStr = GetUserBestScoreForRankingPathStr();
        var rankDisplayNamePathStr = GetUserDisplayNameInBestScoreForRankStr();

        var myBestScore = scoreList.ElementAt(0);
        var displayName = UserManager.Instance.GetDisplayName();
        #endregion

        dataTable.Add(userPathStr, scoreList);
        dataTable.Add(rankPathStr, myBestScore);
        dataTable.Add(rankDisplayNamePathStr, displayName);

        FirebaseDatabaseManager.Instance.UpdateValueAsync(dataTable, (isComplete) => 
        {
            if (isComplete)
            {
                UserManager.Instance.SetScoreList(scoreList);
                isReady?.Invoke();

                CustomDebug.Log($"Save currentScore : {currentScore}");
            }
            else
            {
                CustomDebug.LogError(">>>> SaveCurrentScore Failed");
            }
        });
    }

    private string GetUserScoreListPathStr()
    {
        var userId = UserManager.Instance.GetUserId();
        var specificUserPathStr = DevUtil.Instance.GetDBTargetPathString(EnumSets.DBParentType.Users, userId);
        var userStayingTimePathStr = DevUtil.Instance.GetCombinedPathStr(specificUserPathStr, Constants.DATABASE_SCORE_LIST_PATH_STR);

        return userStayingTimePathStr;
    }

    private string GetUserBestScoreForRankingPathStr()
    {
        var userId = UserManager.Instance.GetUserId();
        var specificUserPathStr = DevUtil.Instance.GetDBTargetPathString(EnumSets.DBParentType.Rank, userId);
        var userStayingTimePathStr = DevUtil.Instance.GetCombinedPathStr(specificUserPathStr, Constants.DATABASE_SCORE_PATH_STR);

        return userStayingTimePathStr;
    }

    private string GetUserDisplayNameInBestScoreForRankStr()
    {
        var userId = UserManager.Instance.GetUserId();
        var specificUserPathStr = DevUtil.Instance.GetDBTargetPathString(EnumSets.DBParentType.Rank, userId);
        var userStayingTimePathStr = DevUtil.Instance.GetCombinedPathStr(specificUserPathStr, Constants.DATABASE_DISPLAY_NAME_PATH_STR);

        return userStayingTimePathStr;
    }

    private void DestroyAllCircles(Action onCompleteDestroyAllCircles = null)
    {
        this.circleManager.DestroyAllCircles(onCompleteDestroyAllCircles);
    }

    private void SpawnNextRandomCircle()
    {
        this.circleManager.TryNextCircleRoutine();

        // this.circleManager.SpawnNextRandomCircle();
    }

    public void AddScore(int score)
    {
        currentScore += score;

        UpdateScoreText(this.currentScore);
    }

    // 동전버전용
    public void CountUpReachedFinalCircle()
    {
        this.reachedFinalCircleCount++;

        this.inGameSceneUiManager.UpdateReachedFinalCircleCount(this.reachedFinalCircleCount);
    }

    public void StartRetryRoutine()
    {
        // Invoke(nameof(InitGameRoutine), 1f);
        InitGameRoutine();
    }

    // 상단 UI, RetryPopUp 에 점수 업데이트
    public void UpdateMyBestScoreUIs()
    {
        FirebaseDatabaseManager.Instance.LoadSpecificUserBestScoreAsync(UserManager.Instance.GetUserId(), (bestScore) => 
        {
            this.inGameSceneUiManager.UpdateBestScoreText(bestScore);
            this.inGameSceneUiManager.UpdateHighestScoreInRetryPopUp(bestScore);
            this.inGameSceneUiManager.UpdateHighestScoreInRankingPopUp(bestScore);
            
        });
    }

    public void ResetData()
    {
        this.currentScore = 0;

        this.reachedFinalCircleCount = 0;
        this.inGameSceneUiManager.UpdateReachedFinalCircleCount(this.reachedFinalCircleCount);

        TouchScreenHandler.Instance.ResetDataOnGameRestart();

        this.circleManager.ResetData();

        this.gameEndCheckModule.ResetData();
    }

    #region UIs
    public void UpdateNextCircleImage(Sprite sprite)
    {
        this.inGameSceneUiManager.UpdateNextCircleImage(sprite);
    }

    public void UpdateScoreText(int score)
    {
        this.inGameSceneUiManager.UpdateScoreText(score);
    }

    public void ShowReachedFinalLevelUis()
    {
        this.inGameSceneUiManager.ShowReachedFinalLevelUis();
    }

    public void MoveFinalLevelCoinToUpperLeftUi(RectTransform target, Action onComplete = null)
    {
        this.inGameSceneUiManager.MoveFinalLevelCoinToUpperLeftUi(target, onComplete);
    }
    #endregion

    #region OnClicks
    public void OnClickRetryBtn()
    {
        DestroyAllCircles(() => 
        {
            ResetData();

            if(this.totalGamePlayCount % 3 == 0 && !UserManager.Instance.GetIsPurchasedRemoveAd())
            {
                // 4판째 하려고 할 때, 7판째 하려고 할 때, 10판째 하려고 할 때…

                // to do : 전면 광고 보여주고, 광고 보고 나면 StartRetryRoutine() 실행하기
                CustomDebug.Log("전면 광고 보여주기!!");
                StartRetryRoutine(); // tmp
            }
            else
            {
                StartRetryRoutine();
            }
        });
    }
    #endregion
}
