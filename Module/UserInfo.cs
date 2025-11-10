using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

// RealtimeDB 신규유저 DB 기초 틀
//public struct DefaultUserInfoTemplate
//{
//    public string userId;
//    public string userEmail;

//    public void SetUserId(string uid)
//    {
//        userId = uid;
//    }

//    public void SetUserEmail(string email)
//    {
//        userEmail = email;
//    }
//}

[Serializable]
public class UserInfo : ISerializable
{
    public string userId = "";
    // public string UserId => this.userId;

    public string displayName = "";

    //public string userEmail = "";
    // public string UserEmail => this.userEmail;

    public List<int> scoreList = new List<int> { 0 };

    public bool isPurchasedRemoveAd = false;

    //private int visitingCount = 0;
    //public int VisitingCount => this.visitingCount;

    //private int stayingTime = 0;
    //public int StayingTime => this.stayingTime;

    public UserInfo()
    {

    }

    // Deserialize
    private UserInfo(SerializationInfo info, StreamingContext context)
    {
        this.userId = info.GetString(nameof(userId));
        //this.userEmail = info.GetString(nameof(userEmail));
        this.scoreList = info.GetValue(nameof(scoreList), this.scoreList.GetType()) as List<int>;
        //this.visitingCount = info.GetInt32(nameof(visitingCount));
        //this.stayingTime = info.GetInt32(nameof(stayingTime));

        //CustomDebug.Log($">>>> UserInfo, id : {userId}, email : {userEmail}"); //, visitingCount : {visitingCount}, stayingTime : {stayingTime}");
    }

    // Serialize
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(userId), this.userId);
        //info.AddValue(nameof(userEmail), this.userEmail);
        //info.AddValue(nameof(visitingCount), this.visitingCount);
        //info.AddValue(nameof(stayingTime), this.stayingTime);
    }

    public void SetUserId(string uuid)
    {
        userId = uuid;
    }

    public void SetUserDisplayName(string nickname)
    {
        this.displayName = nickname;
    }

    //public void SetUserEmail(string email)
    //{
    //    userEmail = email;
    //}

    public void SetScoreList(List<int> scoreList)
    {
        this.scoreList = scoreList;
    }

    public void SetScoreList(List<object> scoreList)
    {
        this.scoreList = new List<int>();

        foreach(var obj in scoreList)
        {
            this.scoreList.Add(Convert.ToInt32(obj));
        }
    }

    public void SetIsUserPurchasedRemoveAd(bool value)
    {
        this.isPurchasedRemoveAd = value;
    }

    //public void SetVisitingCount(int count)
    //{
    //    visitingCount = count;
    //}

    //public void SetStayingTime(int time)
    //{
    //    stayingTime = time;
    //}
}
