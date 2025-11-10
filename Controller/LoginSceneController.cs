using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneController : MonoBehaviour
{
    private static LoginSceneController instance = null;
    public static LoginSceneController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<LoginSceneController>();
            }

            return instance;
        }
    }

    public LoginSceneUiManager uiManager;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        SoundManager.Instance.PlayBGM(0);

        UserManager.Instance.SetSignInSuccessCallback(() =>
        {
            HandleOnAppReady();
        });

        GooglePlayGamesManager.Instance.SetTryAuthenticateGooglePlayGamesCallback(CheckGooglePlayGamesAuthenticateAndFirebaseSignIn);

        GooglePlayGamesManager.Instance.Activate();

        Invoke(nameof(InitAppRoutine), 0.2f);
    }

    private void InitAppRoutine()
    {
        FirebaseAppManager.Instance.CheckAppDependencyAsync(InitializeFirebaseAuth);
    }

    private void InitializeFirebaseAuth(bool isFirebaseDependencyAvailable)
    {
        if (isFirebaseDependencyAvailable)
        {
            CustomDebug.Log("Firebase App is available!!!");

            FirebaseAuthManager.Instance.InitializeFirebaseAuth(() => 
            {
                CustomDebug.Log("<<<< Firebase Auth Initialized >>>>");

                // TryGooglePlayGamesSignIn() 이후에 CheckGooglePlayGamesAuthenticateAndFirebaseSignIn(bool) 실행됨
                GooglePlayGamesManager.Instance.TryGooglePlayGamesSignIn();
            });
        }
        else
        {
            // to do : error 메세지나 팝업 띄워주기
            CustomDebug.Log("Firebase App is not available...");
        }
    }

    private void CheckGooglePlayGamesAuthenticateAndFirebaseSignIn(bool isSuccess)
    {
        if (isSuccess)
        {
            // 구글 플레이 게임즈 로그인에 성공했고, 유저 정보 로드 / 새 유저 정보 쓰기 성공하고 난 뒤 콜백

            // HandleOnAppReady();
        }
        else
        {
            // 구글 플레이 게임즈 로그인에 실패했다

            ShowLoginElements();
        }

        CustomDebug.Log($">>>> CheckGooglePlayGamesAuthenticateAndFirebaseSignIn : {isSuccess}");
    }

    private void PlayInEditor()
    {
        UserManager.Instance.CreateNewUserOrLoadUserWhilePlayingInEditor();

        CustomDebug.Log("PlayInEditor");
    }

    // 로그인 등 모든 준비가 끝났다
    private void HandleOnAppReady()
    {
        StartCheckAnyInputRoutine();

        HideLoginElements();
    }

    private void CheckGameConceptVersion()
    {
        AppInfo.Instance.Init();
    }

    private void StartCheckAnyInputRoutine()
    {
        this.uiManager.ShowPressAnyKeyImage();
        
        StartCoroutine(CorCheckAnyInput());
    }

    private IEnumerator CorCheckAnyInput()
    {
        var isInput = false;

        while (!isInput)
        {
            if (Input.anyKeyDown || Input.touchCount >= 1)
            {
                isInput = true;

                //StopCheckAnyInput();

                HandleOnAnyInputChecked();

                SoundManager.Instance.PlayClickSound();
            }

            yield return null;
        }
    }

    public void HandleOnAnyInputChecked()
    {
        this.uiManager.StartSceneConversionEffect();
    }

    // fadeout oncomplete 에서 참조중
    public void MoveToInGameScene()
    {
        StartCoroutine(CorMoveToInGameScene());
    }

    private IEnumerator CorMoveToInGameScene()
    {
        var async = SceneManager.LoadSceneAsync(1);

        while (!async.isDone)
        {
            // progressHandler.UpdateProgress(async.progress);

            yield return null;
        }
    }

    #region Uis
    public void ShowLoginElements()
    {
        this.uiManager.ShowLoginElements();
    }

    public void HideLoginElements()
    {
        this.uiManager.HideLoginElements();
    }
    #endregion

    #region OnClicks
    public void OnClickSignInGooglePlayGames()
    {
#if UNITY_EDITOR

        PlayInEditor();
#else

        GooglePlayGamesManager.Instance.TryGooglePlayGamesSignInManually();
#endif
    }
    #endregion
}
