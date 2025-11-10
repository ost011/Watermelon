using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameEndCheckModule : MonoBehaviour
{
    public Image imgTopEndLine;
    
    // private CircleModule currentCollidedCircle = null;

    private List<CircleModule> listCurrentTriggeredCircles = new List<CircleModule>();

    private bool isCircleTouchingGameEndChecker = false;

    private Action<bool> checkGameEndCallback = null;

    private IEnumerator checkGameEndEnumerator = null; // 필요 없을듯
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetCheckGameEndCallback(Action<bool> callback)
    {
        this.checkGameEndCallback = callback;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var circleModule = collision.GetComponent<CircleModule>();

        if (circleModule != null && !this.listCurrentTriggeredCircles.Contains(circleModule))
        {
            // this.currentCollidedCircle = circleModule;

            this.listCurrentTriggeredCircles.Add(circleModule);

            CheckGameEndState(circleModule);
            
            // CustomDebug.Log($"collided Game End Check Module : {currentCollidedCircle.name}, {currentCollidedCircle.CircleLevel}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var circleModule = collision.GetComponent<CircleModule>();

        // CustomDebug.Log($"GameEndChecker TriggerExit, {collision.name}");

        if (circleModule != null && this.listCurrentTriggeredCircles.Contains(circleModule)) // circleModule.Equals(currentCollidedCircle))
        {
            // 방금 부딪힌 것이 Exit 했다

            // 1, 2번이 들어왔고 1번만 나간 상황일 때 2번은 체크가 안될듯
            // StopCheckingGameEndState();

            this.listCurrentTriggeredCircles.Remove(circleModule);

            StopShowingWarningEffect();
        }
    }

    private void StopCheckingGameEndState()
    {
        if(this.checkGameEndEnumerator != null)
        {
            StopCoroutine(this.checkGameEndEnumerator);
        }
    }

    // 1번이 닿은 상태에서 2번이 닿았다고 끄면 안됨
    // 1번이 닿고 2번
    private void CheckGameEndState(CircleModule circleModule)
    {
        //if(this.checkGameEndEnumerator == null)
        //{
        //    this.checkGameEndEnumerator = CorCheckGameEndState(circleModule);
        //}
        
        StartCoroutine(CorCheckGameEndState(circleModule));
    }

    // 리팩토링 필요할듯
    private IEnumerator CorCheckGameEndState(CircleModule circleModule)
    {
        // CustomDebug.Log($"CorCheckGameEndState : {circleModule.name}");

        if (this.isCircleTouchingGameEndChecker)
        {
            yield break;
        }

        // 그저 위에서 떨어지는 오브젝트는 0.5 초 전에 떨어진다
        yield return new WaitForSeconds(0.5f);

        if (this.isCircleTouchingGameEndChecker)
        {
            yield break;
        }
        
        if (this.listCurrentTriggeredCircles.Contains(circleModule))
        {
            // 1차 경고, 빨간 선 보여주기
            
            ShowTopEndLineWarningEffect();

            yield return new WaitForSeconds(2.7f);

            if (this.isCircleTouchingGameEndChecker)
            {
                yield break;
            }

            if (this.listCurrentTriggeredCircles.Contains(circleModule))
            {
                // 1차 경고 후에도 선에 오브젝트가 닿아있다
                this.isCircleTouchingGameEndChecker = true;

                StopShowingWarningEffect();
            }
            else
            {
                this.isCircleTouchingGameEndChecker = false;
            }
        }
        else
        {
            this.isCircleTouchingGameEndChecker = false;
        }

        this.checkGameEndCallback.Invoke(this.isCircleTouchingGameEndChecker);

        this.checkGameEndEnumerator = null;
    }

    public bool GetIsGameEnd()
    {
        return this.isCircleTouchingGameEndChecker;
    }

    private void ShowTopEndLineWarningEffect()
    {
        this.imgTopEndLine.DOFade(1f, 0.65f).SetLoops(-1, LoopType.Yoyo);
    }

    private void StopShowingWarningEffect()
    {
        this.imgTopEndLine.DOKill();
        ResetWarningEffect();
    }

    private void ResetWarningEffect()
    {
        this.imgTopEndLine.DOFade(0f, 0f);
    }

    public void ResetData()
    {
        // this.currentCollidedCircle = null;
        this.isCircleTouchingGameEndChecker = false;
        this.checkGameEndEnumerator = null;

        this.listCurrentTriggeredCircles.Clear();
    }
}
