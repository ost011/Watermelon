using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseAppManager
{
    //private static FirebaseAppManager instance = null;
    //public static FirebaseAppManager Instance
    //{
    //    get
    //    {
    //        if(instance == null)
    //        {
    //            instance = FindAnyObjectByType<FirebaseAppManager>();
    //        }

    //        return instance;
    //    }
    //}

    public static FirebaseAppManager Instance;

    static FirebaseAppManager()
    {
        Instance = new FirebaseAppManager();
    }

    private FirebaseAppManager()
    {

    }

    //private FirebaseApp app = null;
    //public FirebaseApp App => this.app;

    public async void CheckAppDependencyAsync(Action<bool> checkDependencyCallback)
    {
        var isReady = false;

        var task = FirebaseApp.CheckAndFixDependenciesAsync();

        await task;

        if (task.IsCompletedSuccessfully)
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus.Equals(DependencyStatus.Available))
            {
                isReady = true;

                //this.app = FirebaseApp.DefaultInstance;

                CustomDebug.Log($"dependencyStatus : {task.Result}");
            }
        }
        else
        {
            if (task.IsCompleted && task.Result.Equals(DependencyStatus.Available))
            {
                isReady = true;

                //this.app = FirebaseApp.DefaultInstance;

                CustomDebug.Log($"{task.Result}");
            }
            else
            {
                CustomDebug.Log($"CheckAppDependency Failed, {task.Result}, {task.Exception}");
            }
        }

        checkDependencyCallback?.Invoke(isReady);


        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        //    var dependencyStatus = task.Result;
        //    if (dependencyStatus == Firebase.DependencyStatus.Available)
        //    {
        //        // Create and hold a reference to your FirebaseApp,
        //        // where app is a Firebase.FirebaseApp property of your application class.
        //        this.app = Firebase.FirebaseApp.DefaultInstance;

        //        // Set a flag here to indicate whether Firebase is ready to use by your app.
        //    }
        //    else
        //    {
        //        UnityEngine.Debug.LogError(System.String.Format(
        //          "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        //        // Firebase Unity SDK is not safe to use here.
        //    }
        //});
    }
}
