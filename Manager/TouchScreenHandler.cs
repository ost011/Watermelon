using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchScreenHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler//, IEndDragHandler
{
    private static TouchScreenHandler instance = null;
    public static TouchScreenHandler Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<TouchScreenHandler>();
            }

            return instance;
        }
    }

    public RectTransform dropCircleGuideImageRt;
    public RectTransform dragLimitRt;

    // public Transform guideImageParent;

    private bool isDraggable = false;
    
    private bool isMousePressed = false;

    private bool isAutoPlay = false;

    [SerializeField]
    private CircleModule currentCircle = null;
    private RectTransform currentCircleRectTransform = null;

    private Action onCurrentCircleDroppedCallback = null; // 어딘가에다가 놨다고 알려주기 => 다음 공을 만들지, 게임 끝인지 등 판단하도록

    private float halfScreenWidth = 0f;

    private Vector2 lastDragPos = Vector2.zero;

    #region 상수값

    private const float CIRCLE_Y_POS_WHILE_DRAG = 545f;// -500f;
    private const float GUIDE_LINE_Y_POS_WHILE_DRAG = -20f;// -550f;
    private const float DRAG_SIDE_LIMIT_X_POS = 490f; // Width : 980, Left : -490, right : 490

    #endregion

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        this.lastDragPos = new Vector2(0, CIRCLE_Y_POS_WHILE_DRAG);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.halfScreenWidth = Screen.width / 2f;
    }

    #region test
    void Update()
    {
        if (isAutoPlay && currentCircleRectTransform != null)
        {
            SetPressedState();

            var randomXPos = UnityEngine.Random.Range(-1f, 1f);

            this.currentCircleRectTransform.anchoredPosition = new Vector2(randomXPos, CIRCLE_Y_POS_WHILE_DRAG);

            currentCircleRectTransform.GetComponent<Rigidbody2D>().gravityScale = 1;
            currentCircleRectTransform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            currentCircleRectTransform.GetComponent<Collider2D>().isTrigger = false;
            currentCircleRectTransform.GetComponent<CircleModule>().SetReadyToMergeState();

            SetUnPressedState();

            ResetData();

            this.onCurrentCircleDroppedCallback?.Invoke();

            SoundManager.Instance.PlaySoundClip(EnumSets.InGameSoundClipType.ReleaseCircle);
        }
    }
    
    public void StartAutoPlay()
    {
        isAutoPlay = true;
    }

    public void StopAutoPlay()
    {
        isAutoPlay = false;
    }
    #endregion

    // 집는 순간 그 위치로 옮긴다
    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.isMousePressed)
        {
            return;
        }

        SetPressedState();

        if (this.isDraggable && this.currentCircle != null && !GetIsDragReachedEndPoint())
        {
            // 드래그 가능 && currentCircle 존재 && 제한 범위 안

            MoveCurrentCircle(eventData);
            MoveDropCircleGuideImage(eventData);

            this.currentCircle.ChangeCircleEmotion(EnumSets.CircleImageTypeBySituation.WhileDragAndDrop);
        }
    }

    // 드래그하는대로 손을 따라온다
    public void OnDrag(PointerEventData eventData)
    {
        if (!this.isDraggable || !this.isMousePressed)
        {
            return;
        }

        if (!GetIsDragReachedEndPoint())
        {
            MoveCurrentCircle(eventData);
            MoveDropCircleGuideImage(eventData);
        }
    }

    // 놓는 위치에 Circle 이 떨어진다
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!this.isDraggable || !this.isMousePressed)
        {
            return;
        }

        if (!GetIsDragReachedEndPoint())
        {
            MoveCurrentCircle(eventData);
            MoveDropCircleGuideImage(eventData);

            SaveLastDragPos(this.dropCircleGuideImageRt.anchoredPosition.x);
        }
        else
        {
            // 제한 범위 밖에서 손을 뗐다,
            // 제한 범위로 오브젝트를 강제로 위치시킬 것
            Vector2 leftLimitPos = new Vector2(Input.mousePosition.x - this.currentCircleRectTransform.rect.width / 2f - this.halfScreenWidth, Input.mousePosition.y);
            Vector2 rightLimitPos = new Vector2(Input.mousePosition.x + this.currentCircleRectTransform.rect.width / 2f - this.halfScreenWidth, Input.mousePosition.y);

            Vector2 tmpPos = Vector2.zero;
            var xPos = 0f;

            if (leftLimitPos.x <= -this.dragLimitRt.rect.width / 2f)
            {
                xPos = -(this.dragLimitRt.rect.width / 2f) + (this.currentCircleRectTransform.rect.width / 2f);
            }
            else
            {
                if(rightLimitPos.x >= this.dragLimitRt.rect.width / 2f)
                {
                    xPos = (this.dragLimitRt.rect.width / 2f) - (this.currentCircleRectTransform.rect.width / 2f);
                }
            }

            tmpPos = new Vector2(xPos, this.currentCircleRectTransform.anchoredPosition.y);

            // CustomDebug.Log($"tmpPos : {tmpPos}, originalMousePos : {Input.mousePosition}");

            this.currentCircleRectTransform.anchoredPosition = tmpPos;
            this.dropCircleGuideImageRt.anchoredPosition = tmpPos;

            SaveLastDragPos(xPos);
        }

        var currentCircleRigidbody2D = currentCircle.GetComponent<Rigidbody2D>();

        currentCircleRigidbody2D.constraints = RigidbodyConstraints2D.None;
        currentCircleRigidbody2D.gravityScale = 1;
        currentCircleRigidbody2D.GetComponent<Collider2D>().isTrigger = false;

        SetUnPressedState();

        this.currentCircle.SetReadyToMergeState();

        // SaveLastDragPos();

        ResetData();

        this.onCurrentCircleDroppedCallback?.Invoke();

        SoundManager.Instance.PlaySoundClip(EnumSets.InGameSoundClipType.ReleaseCircle);
    }

    public void SetDraggableState()
    {
        this.isDraggable = true;
    }

    public void SetUnDraggableState()
    {
        this.isDraggable = false;
    }

    private void SetPressedState()
    {
        this.isMousePressed = true;
    }

    private void SetUnPressedState()
    {
        this.isMousePressed = false;
    }

    // Input.mousePosition : 0~1080
    private bool GetIsDragReachedEndPoint2()
    {
        var isReachedEndPoint = false;

        Vector2 leftLimitPos = new Vector2(Input.mousePosition.x - this.currentCircleRectTransform.rect.width / 2f, Input.mousePosition.y);
        Vector2 rightLimitPos = new Vector2(Input.mousePosition.x + this.currentCircleRectTransform.rect.width / 2f, Input.mousePosition.y);

        //CustomDebug.Log($"left : {leftLimitPos}, right : {rightLimitPos}, originalMousePos : {Input.mousePosition}");

        if (!RectTransformUtility.RectangleContainsScreenPoint(dragLimitRt, leftLimitPos, Camera.main) || !RectTransformUtility.RectangleContainsScreenPoint(dragLimitRt, rightLimitPos, Camera.main))
        {
            isReachedEndPoint = true;

            //CustomDebug.Log($"reached End Point");
        }

        return isReachedEndPoint;
    }

    private bool GetIsDragReachedEndPoint()
    {
        var isReachedEndPoint = false;

        Vector2 leftLimitPos = new Vector2(Input.mousePosition.x - this.currentCircleRectTransform.rect.width / 2f - this.halfScreenWidth, Input.mousePosition.y);
        Vector2 rightLimitPos = new Vector2(Input.mousePosition.x + this.currentCircleRectTransform.rect.width / 2f - this.halfScreenWidth, Input.mousePosition.y);

        // CustomDebug.Log($"left : {leftLimitPos}, right : {rightLimitPos}, originalMousePos : {Input.mousePosition}");

        if (leftLimitPos.x <= -(this.dragLimitRt.rect.width / 2f) || rightLimitPos.x >= (this.dragLimitRt.rect.width / 2f))
        {
            isReachedEndPoint = true;
        }

        return isReachedEndPoint;
    }

    private void MoveCurrentCircle(PointerEventData eventData)
    {
        // this.currentCircleRectTransform.anchoredPosition = new Vector2(eventData.position.x - this.halfScreenWidth, CIRCLE_Y_POS_WHILE_DRAG);
        this.currentCircleRectTransform.anchoredPosition = new Vector2(Input.mousePosition.x - this.halfScreenWidth, CIRCLE_Y_POS_WHILE_DRAG);
    }

    private void MoveDropCircleGuideImage(PointerEventData eventData)
    {
        // this.dropCircleGuideImageRt.anchoredPosition = new Vector2(eventData.position.x - this.halfScreenWidth, GUIDE_LINE_Y_POS_WHILE_DRAG);
        this.dropCircleGuideImageRt.anchoredPosition = new Vector2(Input.mousePosition.x - this.halfScreenWidth, GUIDE_LINE_Y_POS_WHILE_DRAG);
    }

    private void SaveLastDragPos(float xPos)
    {
        this.lastDragPos = new Vector2(xPos, CIRCLE_Y_POS_WHILE_DRAG);
    }

    public Vector2 GetLastDragPos()
    {
        return this.lastDragPos;
    }

    public void ResetData()
    {
        this.currentCircle = null;
        this.currentCircleRectTransform = null;
        //this.dropCircleGuideImageRt.parent = this.guideImageParent.transform;

        // this.dropCircleGuideImageRt.anchoredPosition = new Vector2(0, GUIDE_LINE_Y_POS_WHILE_DRAG);
    }

    public void ResetDataOnGameRestart()
    {
        ResetData();

        SaveLastDragPos(0f);
    }

    public void SetCurrentCircleDroppedCallback(Action callback)
    {
        this.onCurrentCircleDroppedCallback = callback;
    }

    public void SetCurrentDraggableCircle(CircleModule circleModule)
    {
        this.currentCircle = circleModule;
        this.currentCircleRectTransform = circleModule.GetComponent<RectTransform>();

        Vector2 newPos = this.currentCircleRectTransform.anchoredPosition;
        var xPos = newPos.x;

        if (this.dropCircleGuideImageRt.anchoredPosition.x - this.currentCircleRectTransform.rect.width / 2f <= -this.dragLimitRt.rect.width / 2f)
        {
            // 직전에 놓은 물체보다 현재 물체가 더 커서 limitRt 를 삐져나온 상황 : 왼쪽
            xPos = -(this.dragLimitRt.rect.width / 2f) + (this.currentCircleRectTransform.rect.width / 2f);
        }
        else
        {
            if (this.dropCircleGuideImageRt.anchoredPosition.x + this.currentCircleRectTransform.rect.width / 2f >= this.dragLimitRt.rect.width / 2f)
            {
                // 직전에 놓은 물체보다 현재 물체가 더 커서 limitRt 를 삐져나온 상황 : 오른쪽
                xPos = (this.dragLimitRt.rect.width / 2f) - (this.currentCircleRectTransform.rect.width / 2f);
            }
        }

        newPos = new Vector2(xPos, this.currentCircleRectTransform.anchoredPosition.y);

        // CustomDebug.Log($"newPos : {newPos}");

        this.currentCircleRectTransform.anchoredPosition = newPos;

        SynchronizeBothCircleAndGuideImage();
    }

    // 직전에 LimitRt 에 닿은 상태로 놓았고,
    // 이전보다 큰 물체가 생성되면 밀리는 것에 대응하여
    // Circle 과 GuideLine 의 위치를 맞춰준다
    private void SynchronizeBothCircleAndGuideImage()
    {
        if(this.currentCircleRectTransform == null)
        {
            return;
        }

        this.dropCircleGuideImageRt.anchoredPosition = new Vector2(this.currentCircleRectTransform.anchoredPosition.x, GUIDE_LINE_Y_POS_WHILE_DRAG);
    }

    public void ShowDropCircleGuideImage()
    {
        this.dropCircleGuideImageRt.gameObject.SetActive(true);
    }

    public void HideDropCircleGuideImage()
    {
        this.dropCircleGuideImageRt.gameObject.SetActive(false);
    }
}
