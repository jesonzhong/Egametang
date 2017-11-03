using UnityEngine;
using System.Collections;


public static class Globals
{
    public static bool NeedSyncWithServer = false;

    public static string streamingPath;
    public static string persistenPath;

    public static string wwwStreamingPath;
    public static string wwwPersistenPath;

    public static string content;
    public static string title;

    public static bool hasReEnterGameDlg = false;
    public static void Init()
    {
        Globals.streamingPath = Application.streamingAssetsPath;
        Globals.persistenPath = Application.persistentDataPath;

        #if UNITY_ANDROID
        	Globals.wwwStreamingPath = Application.streamingAssetsPath;
			Globals.wwwPersistenPath = "file:///" + Application.persistentDataPath; 
        #else
            Globals.wwwStreamingPath = "file:///" + Application.streamingAssetsPath;
            Globals.wwwPersistenPath = "file:///" + Application.persistentDataPath;
        #endif
    }

}
