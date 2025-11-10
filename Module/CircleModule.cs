using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 내 메인 오브젝트가 되는
/// 수박 (Circle 이라 칭함) 에 붙어있는 모듈 스크립트
/// </summary>
public class CircleModule : MonoBehaviour
{
    //[SerializeField]
    //private Sprite spriteAppear;

    //[SerializeField]
    //private Sprite spriteDragAndDrop;

    //[SerializeField]
    //private Sprite spriteCollided;

    //[SerializeField]
    //private Sprite spriteDefault;

    [SerializeField]
    private Image circleImage = null;

    [SerializeField]
    private EnumSets.CircleLevel circleLevel = EnumSets.CircleLevel.None;
    public EnumSets.CircleLevel CircleLevel => this.circleLevel;

    private Sprite[] wholeCircleImagesBySituation = null;

    private Vector2 posWhenBecomeCurrentCircle = new Vector2(0, -500f);

    private bool isCollidedAtLeastOnce = false; // 다른 물체(벽, 오브젝트) 와 한번이라도 닿았는가
    public bool IsCollidedAtLeastOnce => this.isCollidedAtLeastOnce;

    private bool isReadyToMerge = false; // CurrentCircle 일 때 맨위와 닿는 현상 방지용, 필요 없을듯
    public bool IsReadyToMerge => this.isReadyToMerge;

    private bool isAlreadyMerged = false; // 이미 다른 물체와 합쳐지는 중인가
    public bool IsAlreadyMerged => this.isAlreadyMerged;

    private IEnumerator changeImageEnumerator = null;

    private Action<List<CircleModule>> onCollidedSameCircleCallback = null;
    // private Action<CircleModule> onNewCircleCreatedCallback = null;
    private Action<CircleModule> onCircleDestroyedCallback = null;

    private void Awake()
    {
        if(this.circleImage == null)
        {
            this.circleImage = GetComponent<Image>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetCollidedSameCircleCallback(Action<List<CircleModule>> callback)
    {
        this.onCollidedSameCircleCallback = callback;
    }

    //public void SetOnNewCircleCreatedCallback(Action<CircleModule> callback)
    //{
    //    this.onNewCircleCreatedCallback = callback;
    //}

    public void SetCircleDestroyedCallback(Action<CircleModule> callback)
    {
        this.onCircleDestroyedCallback = callback;
    }

    // CurrentCircle 이 막 된 상황
    public void InitCircle(Action<List<CircleModule>> callback, Sprite[] wholeImages)
    {
        MovePosWhenBecomeCurrentCircle();

        SetCollidedSameCircleCallback(callback);

        this.wholeCircleImagesBySituation = wholeImages;

        ChangeCircleEmotion(EnumSets.CircleImageTypeBySituation.Appear);
    }

    // 같은 레벨의 물체 2개가 합쳐진 상황
    public void InitCircleAfterMerged(Action<List<CircleModule>> callback, Sprite[] wholeImages)
    {
        this.transform.DOScale(0.2f, 0f);
        this.transform.DOScale(1.2f, 0.1f).OnComplete(() =>
        {
            this.transform.DOScale(1f, 0.2f);
        });

        Invoke(nameof(SetReadyToMergeState), 0.05f);

        this.GetComponent<Rigidbody2D>().gravityScale = 1;

        SetCollidedSameCircleCallback(callback);

        this.wholeCircleImagesBySituation = wholeImages;

        ChangeCircleEmotion(EnumSets.CircleImageTypeBySituation.Default);

        EffectManager.Instance.ShowCircleMergeEffect(this.GetComponent<RectTransform>().localPosition, this.circleLevel);
    }

    public void SetReadyToMergeState()
    {
        this.isReadyToMerge = true;
    }

    public void SetNotReadyToMergeState()
    {
        this.isReadyToMerge = false;
    }

    public void SetAlreadyMergedState()
    {
        this.isAlreadyMerged = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var collidedCircleModule = collision.gameObject.GetComponent<CircleModule>();

        var isNeedToChangeImage = true;

        if (collidedCircleModule != null)
        {
            // Circle 끼리 부딪혔다
            // 이 밑으로는 2번 실행되는 구문임

            if (collidedCircleModule.CircleLevel.Equals(this.circleLevel))
            {
                if (this.isAlreadyMerged || collidedCircleModule.IsAlreadyMerged || !this.isReadyToMerge || !collidedCircleModule.IsReadyToMerge)
                {
                    return;
                }

                // 같은 레벨의 Circle 이 부딪혔다

                SetAlreadyMergedState();
                collidedCircleModule.SetAlreadyMergedState();

                List<CircleModule> circleModules = new List<CircleModule>();

                circleModules.Add(this);
                circleModules.Add(collidedCircleModule);

                // this.onCollidedSameCircleCallback?.Invoke(this);
                this.onCircleDestroyedCallback?.Invoke(this);
                this.onCollidedSameCircleCallback?.Invoke(circleModules);

                isNeedToChangeImage = false;

                this.isAlreadyMerged = true;
                // CustomDebug.Log($"같은 레벨의 Circle 이 부딪혔다, {collidedCircleModule.CircleLevel}, anchor : {this.GetComponent<RectTransform>().anchoredPosition}, local : {this.GetComponent<RectTransform>().localPosition}");
            }
        }

        if (isNeedToChangeImage)
        {
            ChangeCircleEmotion(EnumSets.CircleImageTypeBySituation.Collided);

            ChangeCircleToDefaultAfterCollided(1f);
        }

        if (!isCollidedAtLeastOnce)
        {
            isCollidedAtLeastOnce = true;

            SoundManager.Instance.PlaySoundClip(EnumSets.InGameSoundClipType.CircleCollided);
        }
    }

    public void ChangeCircleEmotion(EnumSets.CircleImageTypeBySituation circleImageBySituation)
    {
        var enumIndex = (int)circleImageBySituation;

        this.circleImage.sprite = this.wholeCircleImagesBySituation[enumIndex];
    }

    private void ChangeCircleToDefaultAfterCollided(float delay)
    {
        if(changeImageEnumerator != null)
        {
            StopCoroutine(changeImageEnumerator);

            changeImageEnumerator = null;
        }

        changeImageEnumerator = CorChangeCircleToDefaultAfterCollided(delay);

        StartCoroutine(changeImageEnumerator);
    }

    private IEnumerator CorChangeCircleToDefaultAfterCollided(float delay)
    {
        yield return new WaitForSeconds(delay);

        ChangeCircleEmotion(EnumSets.CircleImageTypeBySituation.Default);
    }

    //public Sprite GetCircleAppearSprite()
    //{
    //    return this.spriteAppear;
    //}

    public void MovePosWhenBecomeCurrentCircle()
    {
        // this.GetComponent<RectTransform>().anchoredPosition = posWhenBecomeCurrentCircle;
        this.GetComponent<RectTransform>().anchoredPosition = TouchScreenHandler.Instance.GetLastDragPos();
    }

    // 지폐 도달 시 아임크리 하트 이펙트 효과 보여주기
    public void StartReachedFinalLevelEffectForCoinVersion()
    {
        //// SetNotReadyToMergeState();
        //Destroy(this.GetComponent<Collider2D>());
        //Destroy(this.GetComponent<Rigidbody2D>());
        //EffectManager.Instance.ShowCircleMergeEffect(this.GetComponent<RectTransform>(), this.circleLevel);

        //// this.GetComponent<Collider2D>().isTrigger = true;

        //InGameSceneController.Instance.MoveFinalLevelCoinToUpperLeftUi(this.GetComponent<RectTransform>(), () => 
        //{
        //    InGameSceneController.Instance.CountUpReachedFinalCircle();

        //    Destroy(this.gameObject);
        //});
    }
}
