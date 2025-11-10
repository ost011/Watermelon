using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
//using Newtonsoft.Json;
using System.Text;
using System.Linq;

public class FirebaseDatabaseManager
{
    public static FirebaseDatabaseManager Instance = null;

    static FirebaseDatabaseManager()
    {
        Instance = new FirebaseDatabaseManager();
    }

    private FirebaseDatabaseManager()
    {
        firebaseDatabase = FirebaseDatabase.GetInstance(dbURL);

        databaseReference = firebaseDatabase.RootReference;

        userDataRef = databaseReference.Child("users");
    }

    private FirebaseDatabase firebaseDatabase = null;
    private DatabaseReference databaseReference = null;

    private string dbURL = "https://watermelon-fruit-default-rtdb.firebaseio.com/";

    private DatabaseReference userDataRef = null;

    private StringBuilder sb = new StringBuilder();

    private const string SLASH_STR = "/";

    public async void IsExistUser(string userId, Action<bool> isExistUser = null)
    {
        var isExist = false;

        Task<DataSnapshot> task = this.userDataRef.Child(userId).GetValueAsync();
        
        await task;

        if (task.IsCompleted)
        {
            DataSnapshot snapShot = task.Result;

            isExist = snapShot.Exists;

            isExistUser?.Invoke(isExist);
        }
        else
        {
            var status = task.Status;
            var exception = task.Exception;

            CustomDebug.LogError($"task is canceled or faulted \n status : {status}, exception : {exception}");
        }
    }

    public async void GetValueAsync(string childPath, Action<DataSnapshot> isComplete = null)
    {
        var task = this.databaseReference.Child(childPath).GetValueAsync();

        await task;

        if (task.IsCompleted)
        {
            var dataSnapShot = task.Result;

            isComplete?.Invoke(dataSnapShot);
        }
        else
        {
            CustomDebug.Log($"task error : {task.Exception.Message}");
        }
    }

    public async void UpdateValueAsync(Dictionary<string, object> table, Action<bool> isComplete = null)
    {
        var task = this.databaseReference.UpdateChildrenAsync(table);

        await task;

        if (task.IsCompleted)
        {
            isComplete?.Invoke(true);
        }
        else
        {
            isComplete?.Invoke(false);
            CustomDebug.Log($"task error : {task.Exception.Message}");
        }
    }

    public async void UpdateValueInSpecificUserAsync(string userId, Dictionary<string, object> table, Action<bool> isComplete = null)
    {
        var task = this.userDataRef.Child(userId).UpdateChildrenAsync(table);

        await task;

        if (task.IsCompleted)
        {
            isComplete?.Invoke(true);
        }
        else
        {
            isComplete?.Invoke(false);
            CustomDebug.Log($"UpdateValueInSpecificUserAsync error : {task.Exception.Message}");
        }
    }

    public async void SetRawJsonValueAsync(string childPath, string json, Action isReady = null)
    {
        var task = this.databaseReference.Child(childPath).SetRawJsonValueAsync(json);

        await task;

        if (task.IsCompleted)
        {
            isReady?.Invoke();
        }
        else
        {
            CustomDebug.LogError(">>>> SetRawJsonValueAsync Error");
        }
    }

    public async void RunTransactionAsync(string targetPath, Action<bool> isComplete = null)
    {
        try
        {
            var isCompleted = true;

            var task = this.databaseReference.Child(targetPath).RunTransaction(transaction =>
            {
                var currentValue = 0;

                if (transaction.Value != null)
                {
                    currentValue = Convert.ToInt32(transaction.Value);
                }

                //if (failCondition)
                //{
                //    CustomDebug.LogError(">>>> transaction failed");

                //    return TransactionResult.Abort();
                //}

                currentValue += 1;

                transaction.Value = currentValue;

                return TransactionResult.Success(transaction);
            });

            await task;

            if (!task.IsCompleted)
            {
                isCompleted = false;
            }

            CustomDebug.Log($"RunTransactionAsync task.IsCompleted : {isCompleted}");

            isComplete?.Invoke(isCompleted);
        }
        catch (Exception e)
        {
            isComplete?.Invoke(false);

            CustomDebug.LogError($"RunTransactionAsync error : {e.Message}");
        }
    }

    public async void LoadRankingDataAsync(Action<Dictionary<string, Dictionary<string, object>>, string> isReady = null)
    {
        CustomDebug.Log(">>>> LoadRankingData Top5 <<<<");
        // databaseRef.Child("leaderboard").OrderByChild("score").LimitToFirst(10).GetValueAsync().ContinueWithOnMainThread(task =>

        // 1, 2, 3, 4, 5, 15, 38 => 3,4,5,15,38 순으로 담고 있음
        // 파베에는 내림차순 정렬이 없다고 함
        var task = databaseReference.Child("rank").OrderByChild(Constants.DATABASE_SCORE_PATH_STR).LimitToLast(5).GetValueAsync();

        await task;

        if (task.IsFaulted || task.IsCanceled)
        {
            // 데이터 로드 실패 처리

            isReady?.Invoke(null, null);
            Debug.LogError("Failed to load leaderboard data.");
        }
        else if (task.IsCompleted || task.IsCompletedSuccessfully)
        {
            CustomDebug.Log("LoadRankingData Task Completed ~~");

            DataSnapshot snapshot = task.Result;

            var userScoreTableOrderedByDescending = new Dictionary<string, int>();
            var top5RankUserDataTable = new Dictionary<string, Dictionary<string, object>>();
            
            string myRankStr = "5+";

            for (int i = snapshot.Children.Count() - 1; i >= 0; i--)
            {
                var specificUserDataTable = new Dictionary<string, object>();

                // 내림차순으로 재정렬하기
                var childSnapshot = snapshot.Children.ElementAt(i);

                var specificUserData = childSnapshot.Value as Dictionary<string, object>;

                var userId = childSnapshot.Key;
                var score = Convert.ToInt32(specificUserData[Constants.DATABASE_SCORE_PATH_STR]);
                var displayName = specificUserData[Constants.DATABASE_DISPLAY_NAME_PATH_STR] as string;

                specificUserDataTable.Add(Constants.DATABASE_SCORE_PATH_STR, score);
                specificUserDataTable.Add(Constants.DATABASE_DISPLAY_NAME_PATH_STR, displayName);

                // userScoreTableOrderedByDescending.Add(userId, score);
                top5RankUserDataTable.Add(userId, specificUserDataTable);

                if (userId.Equals(UserManager.Instance.GetUserId()))
                {
                    // 내가 5위 안에 포함된다

                    myRankStr = (snapshot.Children.Count() - i).ToString();

                    CustomDebug.Log($"my score is in {snapshot.Children.Count() - i}th rank");
                }

                CustomDebug.Log($"LoadRankingData, userId : {userId}, score : {score}");
            }

            isReady?.Invoke(top5RankUserDataTable, myRankStr);
        };
    }

    public async void LoadSpecificUserBestScoreAsync(string userId, Action<int> isReady = null)
    {
        CustomDebug.Log(">>>> LoadSpecificUserBestScoreAsync <<<<");

        var userBestScore = 0;

        var task = databaseReference.Child("rank").GetValueAsync();

        await task;

        if (task.IsFaulted || task.IsCanceled)
        {
            // 데이터 로드 실패 처리

            isReady?.Invoke(userBestScore);
            Debug.LogError($"Failed to load Rank data. userId : {userId}");
        }
        else if (task.IsCompleted || task.IsCompletedSuccessfully)
        {
            DataSnapshot snapshot = task.Result;

            var target = from data in snapshot.Children
                         where data.Key.Equals(userId)
                         select data;

            if(target.Count() > 0)
            {
                // 내 랭킹 데이터가 있다
                var userBestScoreTable = target.First().Value as Dictionary<string, object>;

                userBestScore = Convert.ToInt32(userBestScoreTable[Constants.DATABASE_SCORE_PATH_STR]);
                CustomDebug.Log($"유저 랭킹 데이터가 있다, userId : {userId}, score : {userBestScore}");
            }
            else
            {
                // 유저 랭킹 데이터가 없다
                CustomDebug.Log($"유저 랭킹 데이터가 없다, userId : {userId}");
            }

            isReady?.Invoke(userBestScore);
        };
    }
}
