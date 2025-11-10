using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;
    //private static UserManager instance = null;
    //public static UserManager Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = FindObjectOfType<UserManager>();
    //        }

    //        return instance;
    //    }
    //}

    static UserManager()
    {
        Instance = new UserManager();
    }

    private UserManager()
    {

    }

    private UserInfo userInfo = null;

    //[SerializeField]
    //private UserDataAutoSaveModule userDataAutoSaveModule = null;

    private Action onSignInRoutineFinished = null;


    void Awake()
    {
        Init();
    }

    void OnApplicationQuit()
    {
        StopAutoSaveUserData();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (userInfo != null)
        {
            if (focus)
            {
                StartAutoSaveUserData();
            }
            else
            {
                StopAutoSaveUserData();
            }
        }
    }

    private void Init()
    {
        DontDestroyOnLoad(this.gameObject);

        // userDataAutoSaveModule = GetComponent<UserDataAutoSaveModule>();
    }

    public void SetSignInSuccessCallback(Action callback)
    {
        this.onSignInRoutineFinished = callback;
    }

    public void SetUserId(string uId)
    {
        userInfo.SetUserId(uId);
    }

    //public void SetUserEmail(string email)
    //{
    //    userInfo.SetUserEmail(email);
    //}

    public void SetScoreList(List<int> scoreList)
    {
        userInfo.SetScoreList(scoreList);
    }

    //public void SetUserStayingTime(int time)
    //{
    //    userInfo.SetStayingTime(time);
    //}

    public string GetUserId()
    {
        return userInfo.userId;
    }

    public string GetDisplayName()
    {
        return this.userInfo.displayName;
    }

    //public string GetUserEmail()
    //{
    //    return userInfo.userEmail;
    //}

    public List<int> GetUserScoreList()
    {
        return userInfo.scoreList;
    }

    public bool GetIsPurchasedRemoveAd()
    {
        return this.userInfo.isPurchasedRemoveAd;
    }

    //public int GetUserVisitingCount()
    //{
    //    return userInfo.VisitingCount;
    //}

    //public int GetUserStayingTime()
    //{
    //    return userInfo.StayingTime;
    //}

    /// <summary>
    /// 최초 로그인 or Sign out 이후 재 로그인 시 사용하는 메소드
    /// </summary>
    public void CreateNewUserOrLoadUser(string userId)
    {
        if (!NetworkController.Instance.IsInternetConnected())
        {
            CustomDebug.LogError(">>>> internet is not connected while CreateNewUserOrLoadUser");
            return;
        }

        FirebaseDatabaseManager.Instance.IsExistUser(userId, (isExist) =>
        {
            if (isExist)
            {
                CustomDebug.Log($">>>> CreateNewUserOrLoadUser 기존 사용자 : {userId}");

                LoadUserInfoFromRealtimeDatabase(userId);
            }
            else
            {
                CustomDebug.Log($">>>> CreateNewUserOrLoadUser 새로운 영웅 : {userId}");

                WriteNewUserInRealtimeDatabase(userId);
            }
        });
    }

    public void CreateNewUserOrLoadUserWhilePlayingInEditor()
    {
        var editorTestUserId = "testEditorUserId-Fruit";

        CreateNewUserOrLoadUser(editorTestUserId);
    }

    private void WriteNewUserInRealtimeDatabase(string userId)
    {
        var newUserInfo = new UserInfo();

        newUserInfo.SetUserId(userId);
        newUserInfo.SetUserDisplayName(GooglePlayGamesManager.Instance.GetUserDisplayname());

        // var newUserInfoJson = DevUtil.Instance.GetJson(newUserInfo);
        var newUserInfoJson = JsonUtility.ToJson(newUserInfo, true);

        var newUserChildPathStr = DevUtil.Instance.GetDBTargetPathString(EnumSets.DBParentType.Users, userId);

        FirebaseDatabaseManager.Instance.SetRawJsonValueAsync(newUserChildPathStr, newUserInfoJson, () =>
        {
            this.userInfo = newUserInfo;

            StartAutoSaveUserData();

            onSignInRoutineFinished?.Invoke();

            onSignInRoutineFinished = null;

            CustomDebug.Log(">>>> WriteNewUserInRealtimeDatabase Done >>>>");
        });
    }

    private void LoadUserInfoFromRealtimeDatabase(string userId)
    {
        var currentUserDataPathStr = DevUtil.Instance.GetDBTargetPathString(EnumSets.DBParentType.Users, userId);

        FirebaseDatabaseManager.Instance.GetValueAsync(currentUserDataPathStr, (dataSnapShot) =>
        {
            try
            {
                var tmpUserInfoTable = new Dictionary<string, object>();

                foreach (var data in dataSnapShot.Value as Dictionary<string, object>)
                {
                    tmpUserInfoTable.Add(data.Key, data.Value);
                    // CustomDebug.Log($"dataSnapShot.Value key : {data.Key}, value : {data.Value}");
                }

                var userId = tmpUserInfoTable["userId"] as string;
                var userDisplayName = tmpUserInfoTable[Constants.DATABASE_DISPLAY_NAME_PATH_STR] as string;
                var scoreListAsObject = tmpUserInfoTable[Constants.DATABASE_SCORE_LIST_PATH_STR] as List<object>;
                var scoreList = DevUtil.Instance.GetListOfInt(scoreListAsObject);

                this.userInfo = new UserInfo();
                this.userInfo.SetUserId(userId);
                this.userInfo.SetUserDisplayName(userDisplayName);
                this.userInfo.SetScoreList(scoreList);

                CheckAndFillMissingUserInfos(tmpUserInfoTable, (isSuccess) => 
                {
                    if (isSuccess)
                    {
                        onSignInRoutineFinished?.Invoke();

                        onSignInRoutineFinished = null;

                        CustomDebug.Log($">>>>LoadUserInfoFromRealtimeDatabase, uid : {userInfo.userId}, displayName : {userDisplayName}, scoreListCount : {userInfo.scoreList.Count}");
                    }
                    else
                    {
                        CustomDebug.Log("CheckAndFillMissingUserInfos Failed !!!!");
                    }
                });
            }
            catch (Exception ex)
            {
                CustomDebug.LogError($"LoadUserInfoFromRealtimeDatabase error : {ex.Message}");
            }
        });
    }

    /// <summary>
    /// 현재 유저의 db 와, 최신 개발된 유저의 디폴트db 의 구조가 차이가 나는 지 체크.
    /// 만약 차이가 난다면, 차이나는 요소를 디폴트 값으로 대체해서 세팅한다
    /// </summary>
    private void CheckAndFillMissingUserInfos(Dictionary<string, object> userInfoTable, Action<bool> isReady = null)
    {
        var isSuccessfullyDone = false;
        var isNeedToFillMissingUserInfo = false;

        var defaultCurrentUserPathStr = DevUtil.Instance.GetDBTargetPathString(EnumSets.DBParentType.Users, GetUserId());
        var missingInfoTable = new Dictionary<string, object>();

        var isPurchasedRemoveAd = false;

        if (userInfoTable.ContainsKey(Constants.DATABASE_IS_PURCHASED_REMOVE_AD_PATH_STR))
        {
            isPurchasedRemoveAd = Convert.ToBoolean(userInfoTable[Constants.DATABASE_IS_PURCHASED_REMOVE_AD_PATH_STR]);
        }
        else
        {
            var isPurchasedRemoveAdPathStr = DevUtil.Instance.GetCombinedPathStr(defaultCurrentUserPathStr, Constants.DATABASE_IS_PURCHASED_REMOVE_AD_PATH_STR);

            missingInfoTable.Add(isPurchasedRemoveAdPathStr, false); // default : false

            isNeedToFillMissingUserInfo = true;
        }



        if (isNeedToFillMissingUserInfo)
        {
            // 누락된 유저 정보가 있었다

            FirebaseDatabaseManager.Instance.UpdateValueAsync(missingInfoTable, (isWriteDataSuccess) =>
            {
                if (isWriteDataSuccess)
                {
                    isSuccessfullyDone = true;

                    this.userInfo.SetIsUserPurchasedRemoveAd(isPurchasedRemoveAd);

                    CustomDebug.Log("Write Missing UserInfo Done ~~~");
                }
                else
                {
                    CustomDebug.Log("Failed To Write Missing UserInfo Data...");
                }

                isReady?.Invoke(isSuccessfullyDone);
            });
        }
        else
        {
            // 누락된 유저 정보는 없고, 성공했다고 알림

            this.userInfo.SetIsUserPurchasedRemoveAd(isPurchasedRemoveAd);

            isSuccessfullyDone = true;

            CustomDebug.Log("누락된 정보는 없었고, 성공했다고 알림");
            isReady?.Invoke(isSuccessfullyDone);
        }
    }

    private void CountUpVisitingCount()
    {
        var specificUserPathStr = DevUtil.Instance.GetDBTargetPathString(EnumSets.DBParentType.Users, GetUserId());
        var totalTargetPath = DevUtil.Instance.GetCombinedPathStr(specificUserPathStr, "visitingCount");

        FirebaseDatabaseManager.Instance.RunTransactionAsync(totalTargetPath, (isComplete) =>
        {
            if (isComplete)
            {
                CustomDebug.Log(">>>> Count Up Visiting Count Success");
            }
            else
            {
                CustomDebug.Log(">>>> Count Up Visiting Count Failed");
            }
        });
    }

    public void StartAutoSaveUserData()
    {
        // userDataAutoSaveModule.StartAutoSaveData();
    }

    public void StopAutoSaveUserData()
    {
        // userDataAutoSaveModule.StopAutoSaveData();
    }
}
