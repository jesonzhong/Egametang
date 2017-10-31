using UnityEngine;

#if UNITY_EDITOR
    using GamePrefs = UnityEditor.EditorPrefs;
#else
    using GamePrefs = UnityEngine.PlayerPrefs;
#endif


//默认客户端配置
public class GClientConfig
{
    private static int serverIndex = 0;
    public static int ServerIndex
    {
        set
        {
            serverIndex = value;
            //****tmp
            ServerConfig.LoginServerIP = "192.168.93.61";
        }
        get
        {
            return serverIndex;
        }
    }

    private static string m_serverID;
    public static string ServerID
    {
        get
        {
            return m_serverID;
        }
        set
        {
            m_serverID = value;
        }
    }
    public static bool IsMusicOn = true;
    public static bool IsSoundOn = true;
    public static float MusicVolume = 100.0f;
    public static float SoundVolume = 100.0f;
    public static float MicrophoneVolume = 100.0f;
    public static bool IsAttackLineOn = true;
    public static float ScreenDragSpeed = 0.5f;
    public static float ScreenInertia = 0.85f;
    public static ulong AccountID;

    //是否是高画质
    public static bool IsHighQuality = true;
    //是否设置过画质
    public static bool HasSetQuality = false;

    private static RuntimePlatform m_curPlatform;
    public static RuntimePlatform CurPlatform
    {
        get
        {
            return m_curPlatform;
        }
        set
        {
            m_curPlatform = value;
        }
    }

    public static string GetVersionName()
    {
        return VersionManager.Instance.curVersion;
    }

    public static void Load()
    {
        if (GamePrefs.HasKey("MusicVolume"))
        {
            MusicVolume = GamePrefs.GetFloat("MusicVolume");
        }

        if (GamePrefs.HasKey("SoundVolume"))
        {
            SoundVolume = GamePrefs.GetFloat("SoundVolume");
        }

        if (GamePrefs.HasKey("MicrophoneVolume"))
        {
            MicrophoneVolume = GamePrefs.GetFloat("MicrophoneVolume");
        }

        if (GamePrefs.HasKey("DragSpeed"))
        {
            ScreenDragSpeed = GamePrefs.GetFloat("DragSpeed");
        }

        if (GamePrefs.HasKey("ScreenInertia"))
        {
            ScreenInertia = GamePrefs.GetFloat("ScreenInertia");
        }

        if (GamePrefs.HasKey("Music"))
        {
            IsMusicOn = GamePrefs.GetInt("Music") == 1;
        }

        if (GamePrefs.HasKey("Sound"))
        {
            IsSoundOn = GamePrefs.GetInt("Sound") == 1;
        }

        if (GamePrefs.HasKey("AttackLine"))
        {
            IsAttackLineOn = GamePrefs.GetInt("AttackLine") == 1;
        }


        if (GamePrefs.HasKey("IsHighQuality"))
        {
            IsHighQuality = GamePrefs.GetInt("IsHighQuality") == 1;
        }

        if (GamePrefs.HasKey("HasSetQuality"))
        {
            HasSetQuality = GamePrefs.GetInt("HasSetQuality") == 1;
        }

        

        //AudioManager.Instance.Init();
    }

    public static void Save()
    {
        GamePrefs.SetFloat("DragSpeed", ScreenDragSpeed);
        GamePrefs.SetFloat("ScreenInertia", ScreenInertia);
        GamePrefs.SetFloat("MusicVolume", MusicVolume);
        GamePrefs.SetFloat("SoundVolume", SoundVolume);
        GamePrefs.SetFloat("MicrophoneVolume", MicrophoneVolume);
        GamePrefs.SetInt("Music", IsMusicOn ? 1 : 0);
        GamePrefs.SetInt("Sound", IsSoundOn ? 1 : 0);
        GamePrefs.SetInt("AttackLine", IsAttackLineOn ? 1 : 0);

        GamePrefs.SetInt("IsHighQuality", IsHighQuality ? 1 : 0);
        GamePrefs.SetInt("HasSetQuality", HasSetQuality ? 1 : 0);
    }
}