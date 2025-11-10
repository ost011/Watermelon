using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DevUtil
{
    public static DevUtil Instance = null;

    static DevUtil()
    {
        Instance = new DevUtil();
    }

    private DevUtil()
    {

    }

    private StringBuilder sb = new StringBuilder();
    private System.Random random = new System.Random();

    private const string SLASH_STR = "/";
    private const string USERS_STR = "users";
    private const string RANK_STR = "rank";



    public string GetCombinedPathStr(string root, string subPath)
    {
        sb.Clear();

        sb.Append(root);
        sb.Append(SLASH_STR);
        sb.Append(subPath);

        return sb.ToString();
    }

    public string GetDBTargetPathString(EnumSets.DBParentType dbParentType, string target)
    {
        var parentType = "";

        switch (dbParentType)
        {
            case EnumSets.DBParentType.Users:
                {
                    parentType = USERS_STR;
                }
                break;
            case EnumSets.DBParentType.Rank:
                {
                    parentType = RANK_STR;
                }
                break;
        }

        sb.Clear();

        sb.Append(parentType);
        sb.Append(SLASH_STR);
        sb.Append(target);

        return sb.ToString();
    }

    public int GetCurrentCircleLevelIntValue(EnumSets.CircleLevel circleLevel)
    {
        try
        {
            // CustomDebug.Log($"GetNextCircleLevelValue, {circleLevel}");

            var circleLevelStr = circleLevel.ToString();

            var splitCircleLevelStr = circleLevelStr.Split("_");

            var nextCircleLevelValue = Convert.ToInt32(splitCircleLevelStr[1]) - 1;

            // CustomDebug.Log($"circleLevelStr : {circleLevelStr}, splitCircleLevelStr : {splitCircleLevelStr}, nextCircleLevelValue : {nextCircleLevelValue}");

            return nextCircleLevelValue;
        }
        catch (Exception e)
        {
            CustomDebug.Log($"SpawnTargetCircle error : {e.Message}, {circleLevel}");

            return -1; // dummy
        }
    }

    public int GetNextCircleLevelIntValue(EnumSets.CircleLevel circleLevel)
    {
        try
        {
            // CustomDebug.Log($"GetNextCircleLevelValue, {circleLevel}");

            var circleLevelStr = circleLevel.ToString();

            var splitCircleLevelStr = circleLevelStr.Split("_");

            var nextCircleLevelValue = Convert.ToInt32(splitCircleLevelStr[1]); // 0 번째에 대치되는 것이 level_1 이기 때문

            // CustomDebug.Log($"circleLevelStr : {circleLevelStr}, splitCircleLevelStr : {splitCircleLevelStr}, nextCircleLevelValue : {nextCircleLevelValue}");

            return nextCircleLevelValue;
        }
        catch (Exception e)
        {
            CustomDebug.Log($"SpawnTargetCircle error : {e.Message}, {circleLevel}");

            return -1; // dummy
        }
    }

    public List<int> GetListOfInt(List<object> originalList)
    {
        var resultList = new List<int>();

        foreach (var obj in originalList)
        {
            resultList.Add(Convert.ToInt32(obj));
        }

        return resultList;
    }
}
