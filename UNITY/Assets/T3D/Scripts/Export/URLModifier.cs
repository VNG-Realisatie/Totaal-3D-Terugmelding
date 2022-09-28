using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class URLModifier
{

    public static void SetSessionIdInURL(string sessionId)
    {
#if UNITY_WEBGL && !UNITY_EDITOR

        Unity_URLModifier_SetSessionIdInURL(sessionId);
#else
        Debug.Log("setting ID in session URL only works in a WebGL build");
#endif
    }
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void Unity_URLModifier_SetSessionIdInURL(string sessionID);
#endif
}
