using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleManager : MonoBehaviour
{
    public CircleSpawner circleSpawner;

    //[Space]
    //public CircleInfo circleInfoFruitVer;
    //public CircleInfo circleInfoBankVer;

    [SerializeField]
    private CircleInfo circleInfo = null;

    // private Queue<CircleModule> queueNextCircles = new Queue<CircleModule>();
    private Queue<EnumSets.CircleLevel> queueCircleLevels = new Queue<EnumSets.CircleLevel>();

    private List<CircleModule> listCurrentCollidedCircleModules = new List<CircleModule>();

    [SerializeField]
    private List<CircleModule> listWholeCirclesInStage = new List<CircleModule>();

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // Init();
    }

    public void Init()
    {
//#if COIN_VER && !FRUIT_VER
//        this.circleInfo = this.circleInfoBankVer;
//#elif FRUIT_VER && !COIN_VER
//        this.circleInfo = this.circleInfoFruitVer;
//#endif

        SetCallbacks();
        this.circleSpawner.Init();
    }

    private void SetCallbacks()
    {
        this.circleSpawner.SetOnCollidedSameCircleCallback(OnSameCircleCollided);
        this.circleSpawner.SetOnNewCircleCreatedCallback(AddCircleModuleToList);
        this.circleSpawner.SetCircleDestroyedCallback(RemoveCircleModuleFromList);
    }

    private void AddCircleModuleToList(CircleModule circleModule)
    {
        this.listWholeCirclesInStage.Add(circleModule);
    }

    private void RemoveCircleModuleFromList(CircleModule circleModule)
    {
        if (this.listWholeCirclesInStage.Contains(circleModule))
        {
            this.listWholeCirclesInStage.Remove(circleModule);
        }
        else
        {
            CustomDebug.Log($"{circleModule.name}, {circleModule.CircleLevel} is not contained in CircleManager???");
        }
    }

    //public void SpawnNextRandomCircle()
    //{
    //    var currentSpawnedCircle = this.circleSpawner.GetSpawnedRandomCircle();

    //    this.queueNextCircles.Enqueue(currentSpawnedCircle);

    //    if (this.queueNextCircles.Count < 2)
    //    {
    //        // 하나 더 만들고 방금 것은 current, 하나 더 만드는 것은 next
    //        var nextSpawnedCircle = this.circleSpawner.GetSpawnedRandomCircle();

    //        this.queueNextCircles.Enqueue(nextSpawnedCircle);

    //        currentSpawnedCircle.SetCollidedSameCircleCallback(OnSameCircleCollided);

    //        InGameSceneController.Instance.UpdateNextCircleImage(nextSpawnedCircle.GetCircleAppearSprite());
    //    }
    //    else
    //    {
    //        // Current 는 원래 있었고, 이번에 만든 것이 next 이다

    //        InGameSceneController.Instance.UpdateNextCircleImage(currentSpawnedCircle.GetCircleAppearSprite());
    //    }

    //    // SetCurrentDraggableCircle();

    //    #region 새로 만든 것을 바로 current 에 장착하는 식, old
    //    // this.circleSpawner.SpawnNextRandomCircle();
    //    #endregion
    //}

    public void TryNextCircleRoutine()
    {
        var currentSpawnedCircleLevel = this.circleSpawner.GetRandomCircleLevel();

        this.queueCircleLevels.Enqueue(currentSpawnedCircleLevel);

        if (this.queueCircleLevels.Count < 2)
        {
            // 하나 더 만들고 방금 것은 current, 하나 더 만드는 것은 next

            var nextSpawnedCircleLevel = this.circleSpawner.GetRandomCircleLevel();

            this.queueCircleLevels.Enqueue(nextSpawnedCircleLevel);
        }
        else
        {
            // Current 는 원래 있었고, 이번에 만든 것이 next 이다
        }

        var currentCircleLevel = this.queueCircleLevels.Dequeue();

        var nextCircleLevel = this.queueCircleLevels.Peek();

        var nextCircleImage = this.circleInfo.GetCircleImage(nextCircleLevel, EnumSets.CircleImageTypeBySituation.Appear);

        InGameSceneController.Instance.UpdateNextCircleImage(nextCircleImage);

        InitCurrentCircle(currentCircleLevel);
    }

    private void InitCurrentCircle(EnumSets.CircleLevel currentCircleLevel)
    {
        var currentCircle = this.circleSpawner.GetSpawnedTargetCircle(currentCircleLevel);

        var currentCircleWholeImages = this.circleInfo.GetWholeCircleImagesArray(currentCircleLevel);

        currentCircle.InitCircle(OnSameCircleCollided, currentCircleWholeImages);

        TouchScreenHandler.Instance.SetCurrentDraggableCircle(currentCircle);
    }

    public void OnSameCircleCollided(List<CircleModule> circleModule)
    {
        #region 각자 모듈에서 하나씩 리스트에 추가될 때 로직
        foreach (var circle in circleModule)
        {
            this.listCurrentCollidedCircleModules.Add(circle);
        }

        CheckCanCirclesMerge();
        #endregion

        #region
        //        var currentCircleLevel = circleModule[0].CircleLevel;

        //        var centerPosOfCircles = Vector2.zero;
        //        var localPos = Vector3.zero;

        //        for (int i = circleModule.Count - 1; i >= 0; i--)
        //        {
        //            var collidedCircleModule = circleModule[i];

        //            centerPosOfCircles += (collidedCircleModule.GetComponent<RectTransform>().anchoredPosition / 2f);

        //            localPos += (collidedCircleModule.GetComponent<RectTransform>().localPosition / 2f); // localposition

        //            Destroy(collidedCircleModule.gameObject);
        //        }

        //        // CustomDebug.Log($"final result centerPosOfCircles : {centerPosOfCircles}");

        //        //this.listCurrentCollidedCircleModules.Clear();

        //        // ActivateCircleMergeEffects(localPos, currentCircleLevel);

        //        AddScore(currentCircleLevel);

        //        SoundManager.Instance.PlaySoundClip(EnumSets.InGameSoundClipType.CircleMerged);

        //#if COIN_VER
        //        // 통장 개수 올리기
        //        if (currentCircleLevel.Equals(EnumSets.CircleLevel.Level_10))
        //        {
        //            CustomDebug.Log("최종 레벨을 만들지 않고 상단에 카운트를 올림");

        //            InGameSceneController.Instance.CountUpReachedFinalCircle();

        //            return;
        //        }
        //        // 통장 개수 올리기
        //#endif

        //        if (currentCircleLevel.Equals(EnumSets.CircleLevel.Level_11))
        //        {
        //            CustomDebug.Log("이미 최종 레벨끼리 부딪혔음. 점수는 더 많이 오를 것");

        //            return;
        //        }

        //        var nextLevelIndex = DevUtil.Instance.GetNextCircleLevelIntValue(currentCircleLevel);

        //        // CustomDebug.Log($"currentCircleLevel : {currentCircleLevel}, nextLevelIndex : {nextLevelIndex}");

        //        var wholeNextCircleImages = this.circleInfo.GetWholeCircleImagesArray((EnumSets.CircleLevel)nextLevelIndex);

        //        // CustomDebug.Log($"wholeNextCircleImages.Length : {wholeNextCircleImages.Length}");

        //        this.circleSpawner.SpawnTargetCircle(currentCircleLevel, centerPosOfCircles, wholeNextCircleImages);
        #endregion
    }

    // to do : 가끔 여러개 생성되면서 게임 멈추는 버그 있음, 파악하고 고치기
    private void CheckCanCirclesMerge()
    {
        if(this.listCurrentCollidedCircleModules.Count == 2)
        {
            // 같은 것이 부딪히면 최종적으로 2개가 된다

            var currentCircleLevel = this.listCurrentCollidedCircleModules[0].CircleLevel;

            var centerPosOfCircles = Vector2.zero;
            var localPos = Vector3.zero;

            for (int i = listCurrentCollidedCircleModules.Count - 1; i >= 0; i--)
            {
                var collidedCircleModule = this.listCurrentCollidedCircleModules[i];

                centerPosOfCircles += (collidedCircleModule.GetComponent<RectTransform>().anchoredPosition / 2f);

                localPos += (collidedCircleModule.GetComponent<RectTransform>().localPosition / 2f);

                Destroy(collidedCircleModule.gameObject);
            }

            this.listCurrentCollidedCircleModules.Clear();

            SoundManager.Instance.PlaySoundClip(EnumSets.InGameSoundClipType.CircleMerged);

            AddScore(currentCircleLevel);

            if (currentCircleLevel.Equals(EnumSets.CircleLevel.Level_11))
            {
                CustomDebug.Log("이미 최종 레벨끼리 부딪혔음. 점수는 더 많이 오를 것");

                EffectManager.Instance.ShowCircleMergeEffect(localPos, currentCircleLevel);

                return;
            }

            var nextLevelIndex = DevUtil.Instance.GetNextCircleLevelIntValue(currentCircleLevel);

            var wholeNextCircleImages = this.circleInfo.GetWholeCircleImagesArray((EnumSets.CircleLevel)nextLevelIndex);

            this.circleSpawner.SpawnTargetCircle(currentCircleLevel, centerPosOfCircles, wholeNextCircleImages);
        }
    }

    private void AddScore(EnumSets.CircleLevel circleLevel)
    {
        // Level_1 => 0 을 받는다
        // Score = 2^Index,
        // 최종 레벨은 그것의 x2
        // ex) Lv1 + Lv1 = 1점
        // Lv2 + Lv2 = 2점
        // …
        // Lv11 + Lv11 = 2048 (1024 x 2), 즉 두배
        var circleLevelIndex = DevUtil.Instance.GetCurrentCircleLevelIntValue(circleLevel);

        var score = Math.Pow(2, circleLevelIndex);

        if (circleLevel.Equals(EnumSets.CircleLevel.Level_11))
        {
            score *= 2;
        }

        var scoreIntValue = Convert.ToInt32(score);

        InGameSceneController.Instance.AddScore(scoreIntValue);

        // CustomDebug.Log($"scoreIntValue : {scoreIntValue}");
    }

    // 소리 + 이펙트
    private void ActivateCircleMergeEffects(Vector2 mergePos, EnumSets.CircleLevel circleLevel)
    {
        EffectManager.Instance.ShowCircleMergeEffect(mergePos, circleLevel);
        //var worldPos = Camera.main.ScreenToWorldPoint(mergePos);

        //CustomDebug.Log($"worldPos : {worldPos}, mergePos : {mergePos}");

        //StartCoroutine(CorShowMergeEffect(mergePos));

        SoundManager.Instance.PlaySoundClip(EnumSets.InGameSoundClipType.CircleMerged);
    }

    public void ForceAllCirclesNotToMerge()
    {
        if(this.listWholeCirclesInStage.Count != 0)
        {
            for(int i = this.listWholeCirclesInStage.Count - 1; i >= 0; i--)
            {
                var circleModule = this.listWholeCirclesInStage[i];

                if(circleModule != null)
                {
                    circleModule.SetNotReadyToMergeState();
                }
            }
        }
    }

    public void DestroyAllCircles(Action isReady = null)
    {
        if (this.listWholeCirclesInStage.Count != 0)
        {
            for (int i = this.listWholeCirclesInStage.Count - 1; i >= 0; i--)
            {
                var circleModule = this.listWholeCirclesInStage[i];

                if(circleModule != null)
                {
                    Destroy(circleModule.gameObject);
                }
            }
        }

        isReady?.Invoke();
    }

    public void ResetData()
    {
        this.queueCircleLevels.Clear();

        this.listCurrentCollidedCircleModules.Clear();

        this.listWholeCirclesInStage.Clear();
    }
}
