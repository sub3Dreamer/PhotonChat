using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonDebug
{
    public static void Log(string msg)
    {
#if ENABLE_LOG
        Debug.Log(msg);
#endif
    }

    public static void LogWarning(string msg)
    {
#if ENABLE_LOG
        Debug.LogWarning(msg);
#endif
    }

    public static void LogError(string msg)
    {
#if ENABLE_LOG
        Debug.LogError(msg);
#endif
    }

}
