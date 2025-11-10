
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDebug
{
    static CustomDebug()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
        Debug.Log("This is DEVELOPMENT_BUILD, log Enabled");

#elif USE_DEBUG && UNITY_ANDROID

        Debug.unityLogger.logEnabled = true;
        Debug.Log("This is DEVELOPMENT_BUILD, log Enabled");

#elif USE_DEBUG && UNITY_IOS
        Debug.unityLogger.logEnabled = true;
        Debug.Log("This is DEVELOPMENT_BUILD, log Enabled");

#elif USE_DEBUG && UNITY_WEBGL
        Debug.unityLogger.logEnabled = true;
        Debug.Log("log Enabled");
#else
       // Debug.Log("This is not DEVELOPMENT_BUILD, log Disabled");
        
        Debug.unityLogger.filterLogType = LogType.Exception;
        // Debug.unityLogger.logEnabled = false;

#endif
    }

    public enum ColorSet
    {
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Magenta,
    }

    public static void Log(string msg)
    {
        Debug.Log(msg);
    }

    public static void LogWithColor(string message, ColorSet color)
    {
        var colorOpenTag = "";
        var colorCloseTag = "";

        switch (color)
        {
            case ColorSet.Red:
                {
                    colorOpenTag = "<color=red>";
                    colorCloseTag = "</color>";
                }
                break;
            case ColorSet.Green:
                {
                    colorOpenTag = "<color=green>";
                    colorCloseTag = "</color>";
                }
                break;
            case ColorSet.Blue:
                {
                    colorOpenTag = "<color=blue>";
                    colorCloseTag = "</color>";
                }
                break;
            case ColorSet.Yellow:
                {
                    colorOpenTag = "<color=yellow>";
                    colorCloseTag = "</color>";
                }
                break;
            case ColorSet.Cyan:
                {
                    colorOpenTag = "<color=cyan>";
                    colorCloseTag = "</color>";
                }
                break;
            case ColorSet.Magenta:
                {
                    colorOpenTag = "<color=magenta>";
                    colorCloseTag = "</color>";
                }
                break;
        }

        Debug.Log($"{colorOpenTag}{message}{colorCloseTag}");
    }

    public static void Log(object message)
    {
        Debug.Log($"{message}");
    }

    public static void Log(object message, Object context)
    {

        Debug.Log(message, context);

    }

    public static void LogError(string msg)
    {

        Debug.LogError(msg);

    }

    public static void LogError(object message, Object context)
    {

        Debug.LogError(message, context);

    }

    public static void LogWarning(object message, Object context)
    {

        Debug.LogWarning(message, context);

    }

    public static void LogWarning(object message)
    {

        Debug.LogWarning(message);

    }

    public static void LogException(System.Exception exception, Object context)
    {

        Debug.LogException(exception, context);

    }

    public static void LogException(System.Exception exception)
    {

        Debug.LogException(exception);

    }
}
