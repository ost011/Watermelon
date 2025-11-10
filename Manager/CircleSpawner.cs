using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 게임 내 메인 오브젝트(수박) 을
/// 생성하는 스크립트
/// </summary>
public class CircleSpawner : MonoBehaviour
{
    public CircleModule[] prefabsCircleModule;

    //[Space]
    //public CircleModule[] prefabsCoinVersionCircleModules;

    [Space]
    public Transform circleParent;

    public List<CircleModule> listCircleModules;

    [Range(1, 30)]
    public int poolSize;
    public int poolCursor; // 오브젝트 풀 관리를 위한 사이즈, 커서 변수 추가

    //[SerializeField]
    //private CircleModule[] arrayCircleModulePrefabs = null;

    private int circleIndex = 0; // 몇번째 생성된 오브젝트인가

    // private StringBuilder sb = new StringBuilder();

    private Action<List<CircleModule>> onCollidedSameCircleCallback = null;
    private Action<CircleModule> onNewCircleCreatedCallback = null;
    private Action<CircleModule> onCircleDestroyedCallback = null;

    private const int SPAWN_CIRCLE_LEVEL_LIMIT_INDEX_EXCLUSIVE = 5; // Circle Level => 1~5, Index => 0~4
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        //if (AppInfo.Instance.GameConceptType.Equals(EnumSets.GameConceptType.Fruit))
        //{
        //    this.arrayCircleModulePrefabs = prefabsCircleModule;
        //}
        //else
        //{
        //    this.arrayCircleModulePrefabs = prefabsCoinVersionCircleModules;
        //}
    }

    public void SetOnCollidedSameCircleCallback(Action<List<CircleModule>> callback)
    {
        this.onCollidedSameCircleCallback = callback;
    }

    public void SetOnNewCircleCreatedCallback(Action<CircleModule> callback)
    {
        this.onNewCircleCreatedCallback = callback;
    }

    public void SetCircleDestroyedCallback(Action<CircleModule> callback)
    {
        this.onCircleDestroyedCallback = callback;
    }

    public void SpawnNextRandomCircle()
    {
        try
        {
            var randomPrefabIndex = UnityEngine.Random.Range(0, SPAWN_CIRCLE_LEVEL_LIMIT_INDEX_EXCLUSIVE);
            
            var targetPrefab = prefabsCircleModule[randomPrefabIndex];

            var spawnedCircle = Instantiate(targetPrefab, circleParent);

            this.onNewCircleCreatedCallback?.Invoke(spawnedCircle);

            spawnedCircle.SetCircleDestroyedCallback(this.onCircleDestroyedCallback);
            spawnedCircle.SetCollidedSameCircleCallback(this.onCollidedSameCircleCallback);

            TouchScreenHandler.Instance.SetCurrentDraggableCircle(spawnedCircle);
        }
        catch(Exception e)
        {
            CustomDebug.LogWithColor($"SpawnNextCircle error : {e.Message}", CustomDebug.ColorSet.Red);
        }
    }

    //public CircleModule GetSpawnedRandomCircle()
    //{
    //    var randomPrefabIndex = UnityEngine.Random.Range(0, SPAWN_CIRCLE_LEVEL_LIMIT_INDEX_EXCLUSIVE);

    //    var targetPrefab = prefabsCircleModule[randomPrefabIndex];

    //    var spawnedCircle = Instantiate(targetPrefab, circleParent);

    //    // spawnedCircle.SetCollidedSameCircleCallback(this.onCollidedSameCircleCallback);

    //    return spawnedCircle;
    //}

    public EnumSets.CircleLevel GetRandomCircleLevel()
    {
        var randomPrefabIndex = UnityEngine.Random.Range(0, SPAWN_CIRCLE_LEVEL_LIMIT_INDEX_EXCLUSIVE);

        var randomCircleLevel = (EnumSets.CircleLevel)randomPrefabIndex;

        return randomCircleLevel;
    }

    /// <summary>
    /// CurrentCircle 이 될 물체를 생성할 때 이용
    /// </summary>
    // ObjectPool 이용
    public CircleModule GetCircleModule(EnumSets.CircleLevel circleLevel)
    {
        var nextCirclePrefabIndex = DevUtil.Instance.GetCurrentCircleLevelIntValue(circleLevel);

        if (nextCirclePrefabIndex == -1)
        {
            return null; // dummy
        }

        var targetPrefab = prefabsCircleModule[nextCirclePrefabIndex];

        var spawnedCircle = Instantiate(targetPrefab, circleParent);

        spawnedCircle.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        spawnedCircle.GetComponent<Collider2D>().isTrigger = true;

        this.onNewCircleCreatedCallback?.Invoke(spawnedCircle);

        spawnedCircle.SetCircleDestroyedCallback(this.onCircleDestroyedCallback);

        var originalName = spawnedCircle.name;
        var newName = originalName + circleIndex.ToString();
        spawnedCircle.name = newName;
        this.circleIndex++;

        return spawnedCircle;
    }

    /// <summary>
    /// 같은 레벨의 물체 2개가 닿았을 때 다음 물체를 그자리에 생성할 때 이용
    /// </summary>
    // ObjectPool 이용
    public void PoolTargetCircle(EnumSets.CircleLevel circleLevel, Vector2 centerAnchoredPos, Sprite[] wholeImages)
    {
        var nextCirclePrefabIndex = DevUtil.Instance.GetNextCircleLevelIntValue(circleLevel);

        if (nextCirclePrefabIndex == -1)
        {
            return;
        }

        var targetPrefab = prefabsCircleModule[nextCirclePrefabIndex];

        var spawnedCircle = Instantiate(targetPrefab, circleParent);

        var originalName = spawnedCircle.name;
        var newName = originalName + "_" + circleIndex.ToString();
        spawnedCircle.name = newName;
        this.circleIndex++;

        this.onNewCircleCreatedCallback?.Invoke(spawnedCircle);

        spawnedCircle.SetCircleDestroyedCallback(this.onCircleDestroyedCallback);

        spawnedCircle.GetComponent<RectTransform>().anchoredPosition = centerAnchoredPos;

        spawnedCircle.InitCircleAfterMerged(this.onCollidedSameCircleCallback, wholeImages);
    }

    #region Instantiate 이용, Obsolete
    /// <summary>
    /// CurrentCircle 이 될 물체를 생성할 때 이용
    /// </summary>
    public CircleModule GetSpawnedTargetCircle(EnumSets.CircleLevel circleLevel)
    {
        var nextCirclePrefabIndex = DevUtil.Instance.GetCurrentCircleLevelIntValue(circleLevel);

        if (nextCirclePrefabIndex == -1)
        {
            return null; // dummy
        }

        // nextCirclePrefabIndex = 9; // tmp

        var targetPrefab = prefabsCircleModule[nextCirclePrefabIndex];

        var spawnedCircle = Instantiate(targetPrefab, circleParent);

        spawnedCircle.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        spawnedCircle.GetComponent<Collider2D>().isTrigger = true;

        this.onNewCircleCreatedCallback?.Invoke(spawnedCircle);

        spawnedCircle.SetCircleDestroyedCallback(this.onCircleDestroyedCallback);

        var originalName = spawnedCircle.name;
        var newName = originalName + circleIndex.ToString();
        spawnedCircle.name = newName;
        this.circleIndex++;

        return spawnedCircle;
    }

    /// <summary>
    /// 같은 레벨의 물체 2개가 닿았을 때 다음 물체를 그자리에 생성할 때 이용
    /// </summary>
    public void SpawnTargetCircle(EnumSets.CircleLevel circleLevel, Vector2 centerAnchoredPos, Sprite[] wholeImages)
    {
        var nextCirclePrefabIndex = DevUtil.Instance.GetNextCircleLevelIntValue(circleLevel);

        if(nextCirclePrefabIndex == -1)
        {
            return;
        }

        var targetPrefab = prefabsCircleModule[nextCirclePrefabIndex];

        var spawnedCircle = Instantiate(targetPrefab, circleParent);
        
        var originalName = spawnedCircle.name;
        var newName = originalName + "_" + circleIndex.ToString();
        spawnedCircle.name = newName;
        this.circleIndex++;

        this.onNewCircleCreatedCallback?.Invoke(spawnedCircle);

        spawnedCircle.SetCircleDestroyedCallback(this.onCircleDestroyedCallback);

        spawnedCircle.GetComponent<RectTransform>().anchoredPosition = centerAnchoredPos;

        spawnedCircle.InitCircleAfterMerged(this.onCollidedSameCircleCallback, wholeImages);
    }
    #endregion
}
