using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using System;

public class GooglePlayGamesManager : MonoBehaviour
{
    //private static GooglePlayGamesManager instance = null;
    //public static GooglePlayGamesManager Instance
    //{
    //    get
    //    {
    //        if(instance == null)
    //        {
    //            instance = FindObjectOfType<GooglePlayGamesManager>();
    //        }

    //        return instance;
    //    }
    //}

    public static GooglePlayGamesManager Instance;

    static GooglePlayGamesManager()
    {
        Instance = new GooglePlayGamesManager();
    }

    private GooglePlayGamesManager()
    {
        // Init();
    }

    private Action<bool> onTryAuthenticateGooglePlayGames = null;

    public void Activate()
    {
        PlayGamesPlatform.Activate();
        CustomDebug.Log("PlayGamesPlatform.Activate(); ~~~!~!~!~!");
    }

    public void SetTryAuthenticateGooglePlayGamesCallback(Action<bool> callback)
    {
        this.onTryAuthenticateGooglePlayGames = callback;
    }

    public void TryGooglePlayGamesSignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }
        else
        {
            this.onTryAuthenticateGooglePlayGames?.Invoke(true);

            CustomDebug.Log("TryGooglePlayGamesSignIn Already Authenticated");
        }
    }

    // LoginSceneController.Instance.OnClickSignInGooglePlayGames() 에서 호출중
    public void TryGooglePlayGamesSignInManually()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
        }
        else
        {
            this.onTryAuthenticateGooglePlayGames?.Invoke(true);

            CustomDebug.Log("TryGooglePlayGamesSignInManually Already Authenticated");
        }
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        var isSuccess = false;
        
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            CustomDebug.Log("<<<< ProcessAuthentication, status == SignInStatus.Success >>>>");

            PlayGamesPlatform.Instance.RequestServerSideAccess(true, (code) =>
            {
                CustomDebug.Log($">>>> Authorization code: : {code}");

                FirebaseAuthManager.Instance.FirebaseSignInWithGoogleIdToken(code, (isSignInSuccess) => 
                {
                    this.onTryAuthenticateGooglePlayGames?.Invoke(isSignInSuccess);
                });

                // This token serves as an example to be used for SignInWithGooglePlayGames
            });
        }
        else
        {
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).

            this.onTryAuthenticateGooglePlayGames?.Invoke(isSuccess);
            CustomDebug.Log($"ProcessAuthentication Failed, {status}");
        }
    }

    public string GetUserDisplayname()
    {
        if (Application.isEditor)
        {
            return "editorDisplayName";
        }
        else
        {
            var userDisplayName = PlayGamesPlatform.Instance.GetUserDisplayName();
            var userId = PlayGamesPlatform.Instance.GetUserDisplayName();

            CustomDebug.Log($"userDisplayName : {userDisplayName}, userId : {userId}");
            return userDisplayName;
        }
    }

    [Obsolete]
    public void SignOut(Action isComplete = null)
    {
        var platform = Social.Active as PlayGamesPlatform;

        if (platform != null)
        {
            // platform.SignOut(); // 없음

            isComplete?.Invoke();
            
            CustomDebug.Log(">>>> No Google SignOut");
        }
    }

    // ---------------------------------------
    // Google Play Games Plugin v 11.01
    public void LoginGooglePlayGames(Action onAuthenticateSuccess = null, Action onAutenticateFail = null)
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate((status) =>
            {
                if (status == SignInStatus.Success)
                {
                    // var user = Social.localUser as PlayGamesLocalUser;

                    CustomDebug.Log("<<<< LoginGooglePlayGames, status == SignInStatus.Success >>>>");
                    
                    PlayGamesPlatform.Instance.RequestServerSideAccess(true, (code) =>
                    {
                        CustomDebug.Log($">>>> Authorization code: : {code}");

                        FirebaseAuthManager.Instance.FirebaseSignInWithGoogleIdToken(code);

                        onAuthenticateSuccess?.Invoke();

                        // This token serves as an example to be used for SignInWithGooglePlayGames
                    });
                }
                else
                {
                    onAutenticateFail?.Invoke();

                    CustomDebug.LogError(">>>> LoginGooglePlayGames Error");
                }
            });
        }
        else
        {
            onAuthenticateSuccess?.Invoke(); // Firebase SignIn 은 따로 해야하는지 확인 필요

            CustomDebug.Log("LoginGooglePlayGames, already authenticated");
        }
    }

    private void ProcessAuthentication2(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services

            PlayGamesPlatform.Instance.RequestServerSideAccess(true, (code) =>
            {
                CustomDebug.Log($">>>> Authorization code: : {code}");

                FirebaseAuthManager.Instance.FirebaseSignInWithGoogleIdToken(code);

                // This token serves as an example to be used for SignInWithGooglePlayGames
            });
        }
        else
        {
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).

            CustomDebug.LogError(">>>> LoginGooglePlayGames Unsuccessful, Failed to retrieve Google play games authorization code");
        }
    }

    #region Google Play Games Plugin v10.14
    //private void Init()
    //{
    //    var confing = new PlayGamesClientConfiguration.Builder()
    //        .RequestEmail()
    //        .RequestIdToken()
    //        .Build();

    //    PlayGamesPlatform.InitializeInstance(confing);
    //    PlayGamesPlatform.DebugLogEnabled = true;

    //    PlayGamesPlatform.Activate();
    //}

    //public void DoGoogleSignIn()
    //{
    //    if(!PlayGamesPlatform.Instance.localUser.authenticated)
    //    {
    //        Social.localUser.Authenticate((success, error) => { 

    //            if(success)
    //            {
    //                var user = Social.localUser as PlayGamesLocalUser;

    //                var idToken = user.GetIdToken();

    //                // CustomDebug.Log($"user.GetIdToken() : {user.GetIdToken()}");

    //                FirebaseAuthController.Instance.FirebaseSignInWithGoogleIdToken(user.GetIdToken());

    //                // this.info.text = $"{Social.localUser.id} / {Social.localUser.userName} / {user.GetIdToken()} / {user.Email}";
    //            }
    //            else
    //            {
    //                CustomDebug.LogError("Google Login Error >>");

    //                // this.info.text = $"Failed / {error}";
    //            }
    //        });
    //    }
    //}

    //private void ProcessAuthentication(SignInStatus status)
    //{
    //    if (status == SignInStatus.Success)
    //    {
    //        // Continue with Play Games Services

    //        var localUser = PlayGamesPlatform.Instance.localUser;

    //        // this.info.text = $"Manual, {localUser.id} / {localUser.userName}";
    //    }
    //    else
    //    {
    //        // this.info.text = $"second, {status}";

    //        // Disable your integration with Play Games Services or show a login button
    //        // to ask users to sign-in. Clicking it should call
    //        // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
    //    }
    //}

    //// Update is called once per frame
    //public void SignOut(Action isComplete = null)
    //{
    //    var platform = Social.Active as PlayGamesPlatform;

    //    if(platform != null)
    //    {
    //        platform.SignOut();

    //        isComplete?.Invoke();

    //        CustomDebug.Log(">>>> Google SignOut");

    //        // this.info.text = $"SignOut success";
    //    }
    //    else
    //    {
    //        // this.info.text = $"SignOut failed";
    //    }
    //}
    #endregion
}
