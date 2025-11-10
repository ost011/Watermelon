using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuthManager
{
    public static FirebaseAuthManager Instance;

    static FirebaseAuthManager()
    {
        Instance = new FirebaseAuthManager();
    }

    private FirebaseAuthManager()
    {

    }

    ~FirebaseAuthManager()
    {
        auth.StateChanged -= AuthStateChanged;

        auth = null;
        user = null;

        isInitialized = false;
    }

    private FirebaseAuth auth = null;
    private FirebaseUser user = null;

    private bool authSignedIn = false;
    public bool AuthSignedIn => this.authSignedIn;

    private bool isInitialized = false;

    private Credential pendingCredential = null;

    public Action signOutEvent = null;
    public Action signOutForDropOutEvent = null;


    public void InitializeFirebaseAuth(Action isFirebaseInitialized = null)
    {
        if (!isInitialized)
        {
            // CustomDebug.Log("Initialize Firebase Auth -----");

            isInitialized = true;

            this.auth = FirebaseAuth.DefaultInstance;

            auth.StateChanged += AuthStateChanged;

            isFirebaseInitialized?.Invoke();
        }
    }

    private void AuthStateChanged(object sender, EventArgs e)
    {
        if (auth.CurrentUser != user)
        {
            authSignedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!authSignedIn && user != null) // 이전 auth 인증이 sign out 됨 / 다른 계정으로 접속하려함
            {
                CustomDebug.Log("Signed out " + user.UserId);

                HandleOnSignOut();

                //GoogleLoginController.Instance.SignOut(() =>
                //{
                //    HandleOnSignOut();
                //});
            }

            user = auth.CurrentUser;

            if (authSignedIn) // 이전 auth 인증정보로 계속 진행함
            {
                CustomDebug.Log("Signed in " + user.UserId);

                HandleOnAuthSignedIn();
            }
        }
    }

    private void HandleOnAuthSignedIn()
    {
        //LoginSceneController.Instance.DeActivateLoginButtons();
        //LoginSceneController.Instance.ActivateSignOutButton();

        UserManager.Instance.CreateNewUserOrLoadUser(user.UserId);
    }

    private void HandleOnSignOut()
    {
        //LoginSceneController.Instance.DeActivateSignOutButton();
        //LoginSceneController.Instance.ActivateLoginButtons();

        UserManager.Instance.StopAutoSaveUserData();
    }

    public void SignOutFirebaseAuth()
    {
        if (auth != null)
        {
            auth.SignOut();
        }

#if UNITY_EDITOR

        HandleOnSignOut();
#endif
    }

    public async void FirebaseSignInWithGoogleIdToken(string idToken, Action<bool> isReady = null)
    {
        try
        {
            var googleIdToken = idToken;

            // var credential = GoogleAuthProvider.GetCredential(googleIdToken, null);
            var credential = PlayGamesAuthProvider.GetCredential(googleIdToken);

            // AuthenticationService.Instance.SignInWithGooglePlayGamesAsync()

            var task = auth.SignInWithCredentialAsync(credential);

            await task;

            if (task.IsCanceled || task.IsFaulted)
            {
                isReady?.Invoke(false);
                CustomDebug.Log($"firebase login task error : {task.Exception.Message}");
            }
            else
            {
                isReady?.Invoke(true);
                CustomDebug.Log("FirebaseSignInWithGoogleIdToken Completed~~");
            }
        }
        catch (Exception e)
        {
            CustomDebug.LogError($"firebase login error : {e.Message}");

            if (e.InnerException != null)
            {
                CustomDebug.LogError($"error inner Exception : {e.InnerException.Message}");
            }

            isReady?.Invoke(false);
        }
    }

    public string GetUserSignedInEmail()
    {
        if(this.user == null)
        {
            return "TestEmail@gmail.com";
        }
        else
        {
            return this.user.Email;
        }
    }
}
