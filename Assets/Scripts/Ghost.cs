using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ghost {

    static Ghost()
    {
        #if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
            IsDebug = true;
        #else
            IsDebug = false;
        #endif
    }
    
    public static bool IsDebug { get; private set; }
    public static string Session { get; set; }

    public static float Latency;

    public static void Log(string message)
    {
        if (IsDebug)
            Debug.Log(message);
    }
    
    public static void Log(object message)
    {
        if (IsDebug)
            Debug.Log(message);
    }
}
