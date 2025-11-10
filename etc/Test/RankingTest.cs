using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingTest : MonoBehaviour
{
    public Text leaderboardText; // 리더보드를 표시할 Text
    public Text myRankText; // 내 랭킹을 표시할 Text

    private DatabaseReference databaseRef; // Firebase Realtime Database 레퍼런스

    //private FirebaseDatabaseManager()
    //{
    //    firebaseDatabase = FirebaseDatabase.GetInstance(dbURL);

    //    databaseReference = firebaseDatabase.RootReference;

    //    userDataRef = databaseReference.Child("users");
    //}

    //private FirebaseDatabase firebaseDatabase = null;
    //private DatabaseReference databaseReference = null;

    private string dbURL = "https://watermelon-fruit-default-rtdb.firebaseio.com/";

    private void Start()
    {
        // Firebase Realtime Database 레퍼런스 초기화
        databaseRef = FirebaseDatabase.GetInstance(dbURL).RootReference;

        // 리더보드 데이터 로드 및 UI 업데이트
        LoadLeaderboardData();
    }

    private void LoadLeaderboardData()
    {
        CustomDebug.Log($"Test LoadLeaderboardData >>>>");
        
        // 리더보드 데이터 조회
        // databaseRef.Child("leaderboard").OrderByChild("score").LimitToFirst(10).GetValueAsync().ContinueWithOnMainThread(task =>
        databaseRef.Child("rank").OrderByChild("score").LimitToFirst(5).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // 데이터 로드 실패 처리
                Debug.LogError("Failed to load leaderboard data.");
            }
            else if (task.IsCompleted || task.IsCompletedSuccessfully)
            {
                // 리더보드 데이터 로드 성공
                DataSnapshot snapshot = task.Result;
                string leaderboardData = "";
                
                int rank = 1;
                foreach (var childSnapshot in snapshot.Children)
                {
                    string username = childSnapshot.Child("username").Value.ToString();
                    int score = int.Parse(childSnapshot.Child("score").Value.ToString());

                    leaderboardData += rank + ". " + username + ": " + score + "\n";
                    rank++;
                    CustomDebug.Log($"userName : {username}, score : {score}");
                }

                // 리더보드 텍스트 업데이트
                // leaderboardText.text = leaderboardData;
                CustomDebug.Log($"leaderboardDataStr : {leaderboardData}");

                // 내 랭킹 데이터 로드 및 업데이트
                //LoadMyRankData();
            }
        });
    }

    private void LoadMyRankData()
    {
        // 사용자의 닉네임을 기반으로 현재 랭킹 데이터 조회
        string myNickname = "사용자의 닉네임을 가져오는 로직을 구현해야 합니다."; // 사용자의 닉네임을 가져와야 합니다.

        // 사용자의 닉네임을 기준으로 데이터 조회
        var query = databaseRef.Child("leaderboard").OrderByChild("username").EqualTo(myNickname);
        
        // 쿼리 실행하여 결과 가져오기
        query.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // 데이터 로드 실패 처리
                Debug.LogError("Failed to load my rank data.");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // 사용자의 랭킹 데이터 가져오기
                if (snapshot.HasChildren)
                {
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        int myRank = int.Parse(childSnapshot.Child("rank").Value.ToString());

                        // 내 랭킹 텍스트 업데이트
                        myRankText.text = "내 랭킹: " + myRank;
                    }
                }
                else
                {
                    // 사용자 데이터가 없는 경우 처리
                    myRankText.text = "내 랭킹: 데이터 없음";
                }
            }
        });
    }
}
