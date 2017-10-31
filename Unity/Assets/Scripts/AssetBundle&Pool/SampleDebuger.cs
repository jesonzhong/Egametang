using UnityEngine;
using Junfine.Debuger;

/// <summary>
/// Log 封装类
/// </summary>
public class SampleDebuger {

    public static int ConnectLogID = 0;

#if RELEASE_VER
	static private bool enableLog = false;
	static private bool enableLogAll = false;//启用所有log
#else
    static private bool enableLog = true;
    static private bool enableLogAll = true; //启用所有log
#endif
    static private bool enbaleOneTimeLog = true;
    static private bool colorOneTimeLog = true;
    static private bool enableLogError = false;
    static private bool enableLogWarning = false;

    static private bool bInited = false;

    static public void loadConfig()
    {
        bInited = true;
#if RELEASE_VER
        enableLog = MassiveConfig.Instance.enableLog;
        enableLogAll = MassiveConfig.Instance.enableLogAll;
        enableLogError = MassiveConfig.Instance.enableLogError;
        enableLogWarning = MassiveConfig.Instance.enableLogWarning;
        enbaleOneTimeLog = MassiveConfig.Instance.enbaleOneTimeLog;
        colorOneTimeLog = MassiveConfig.Instance.colorOneTimeLog;
#endif
    }

    static public void LogConnect(object message)
    {
        SampleDebuger.ConnectLogID++;

        string msg = SampleDebuger.ConnectLogID.ToString() + " @@@connect " + message;

        OneTimeLog(msg);
    }

    static public void OneTimeLog(object message)
    {
        if (!bInited)
            loadConfig();

        if (enbaleOneTimeLog)
        {
            if (colorOneTimeLog)
                Debuger.Log(string.Format("<color={0}>{1} - {2}</color>", "green", message.ToString(), System.DateTime.Now), "");
            else
                Debuger.Log(message.ToString() + " " + System.DateTime.Now, "");
            
        }
    }
    static public void Log(object message) {
        if (!bInited)
            loadConfig();

        if (enableLogAll && enableLog) {
            Debuger.Log(message.ToString(), "");
        }
    }
          
    static public void Log(object message, Object context) {
        if (!bInited)
            loadConfig();

        if (enableLogAll && enableLog) {
            Debuger.Log(message.ToString(), context);
        }

    }

    static public void LogColor(object message, string color = "green") {
        if (!bInited)
            loadConfig();

        if (enableLogAll && enableLog) {
            Debuger.Log(string.Format("<color={0}>{1} - {2}</color>", color, message.ToString(), System.DateTime.Now), "");
        }
    }

    static public void LogGreen(object message)
    {
        SampleDebuger.LogColor(message, "green");
    }

    static public void LogRed(object message)
    {
        SampleDebuger.LogColor(message, "red");
    }

    static public void LogBlue(object message)
    {
        SampleDebuger.LogColor(message, "blue");
    }

    static public void LogError(object message) 
    {
        if (!bInited)
            loadConfig();

        if (enableLogAll && enableLog) {
            Debuger.LogError(message.ToString(), "");
        }
    }

    static public void LogError(object message, Object context) {
        if (!bInited)
            loadConfig();

        if (enableLogAll && enableLogError) {
            Debuger.LogError(message.ToString(), context);
        }
    }

    static public void LogWarning(object message) {
        if (!bInited)
            loadConfig();

        if (enableLogAll && enableLogError) {
            Debuger.LogWarning(message.ToString(), "");
        }
    }

    static public void LogWarning(object message, Object context) {
        if (!bInited)
            loadConfig();

        if (enableLogAll && enableLogWarning) {
            Debuger.LogWarning(message.ToString(), context);
        }
    }

}
