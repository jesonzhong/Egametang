using UnityEngine;
using System.Collections;

/// <summary>
/// 服务器配置信息
/// </summary>
public class ServerConfig
{
    static ServerConfig _instance;

    public static string LoginServerIP;
    public static ServerConfig Instance
    {
        get
        {
            if (_instance == null)
            {
			#if !RELEASE_VER
                #if UNITY_IOS && !UNITY_EDITOR
                    _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_localServerIOS.json");
                #elif UNITY_ANDROID && !UNITY_EDITOR
                    _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_localServerAPK.json");
                #else
                    _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_localServer.json");
                #endif
                
            #elif STORE_VERSION
                #if UNITY_IOS && !UNITY_EDITOR
                    _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_storeServerIOS.json");
                #elif UNITY_ANDROID && !UNITY_EDITOR
                    _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_storeServerAPK.json");
                #else
                    _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_storeServer.json");
                #endif

            #else
                #if UNITY_IOS && !UNITY_EDITOR
                    _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_betaServerIOS.json");
                #elif UNITY_ANDROID && !UNITY_EDITOR
                    _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_betaServerAPK.json");
                    string channel = ChannelManager.Instance.GetChannel();
                    if (channel == "jufengtest")
                    {
                        _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_tmpServerAPK.json");
                    }
                    else
                    {
                        _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_betaServerAPK.json");
                    }
                #else
                    _instance = FileUtil.ReadJsonFromFile<ServerConfig>("Config/cfg_betaServer.json");
                #endif

            #endif

            #if RELEASE_VER
                    string[] ips = _instance.LoginServerIPArray.Split(';');
                    if (ips.Length > 0)
                    {
                        ServerConfig.LoginServerIP = ips[UnityEngine.Random.Range(0, ips.Length - 1)];
                        Debug.Log("@@@ServerConfig.LoginServerIP: " + ServerConfig.LoginServerIP);
                    }
            #endif
            }

            return _instance;
        }

    }

    public string NoticeUrl;

    public string LoginServerIPArray;

    public int LoginServerPort;

    public string IMServerIP;

    public int IMServerPort;

    public string PatchUrl;

    public string InstallUrl;


}
