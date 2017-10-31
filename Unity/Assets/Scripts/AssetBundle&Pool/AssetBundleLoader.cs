using UnityEngine;
using System;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
    using UnityEditor;
#endif


public class AssetBundleLoader : MonoBehaviour
{
    public string[] preloadBundles;
    const string kAssetBundlesPath = "/AssetBundles/";

    //开始场景ab包配置
    [SerializeField]
    public string sceneAssetBundle;
    
    [SerializeField]
    public string sceneName;

    private string sceneBundlePath = "assets/abres/scene/";
    private bool firstLoaded = false;
    private bool isResPreloaded = false;

    static AssetBundleLoader _instance;
    public static AssetBundleLoader Instance
    {
        get
        {
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    void Awake()
    {
        AssetBundleLoader.Instance = this;
        DontDestroyOnLoad(gameObject);
        Globals.Init();

        is_scene_loaded_ = false;
        name_scene_loaded_ = string.Empty;
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(CheckVersion());
        yield return StartCoroutine(Initialize());
        yield return new WaitForSeconds(2.0f);
        if (!firstLoaded && !string.IsNullOrEmpty(sceneName))
        {
            LoadLevelAsset(sceneName);
            firstLoaded = true;
        }
        EventDispatcher.Instance.Emit("ABRES_INIT_DONE");
    }

    public IEnumerator Initialize(Action cb=null)
    {
        var request = AssetBundleManager.Initialize(getBundleDirName());
		if (request != null){
			Debug.Log(" loading  manifest ");
            yield return StartCoroutine(request);
		}
        if (cb!=null)
        {
            cb();
        }
    }

    IEnumerator CheckVersion()
    {
        string path = Globals.wwwPersistenPath + "/version.txt";
        WWW www = new WWW(path);
        yield return www;
        string oldVersionStr = www.text.Trim();       
        GameVersion oldVersion = new GameVersion(oldVersionStr);
        www = new WWW(Globals.wwwStreamingPath + "/version.txt");
        yield return www;
        string curVersionStr = www.text.Trim();
        GameVersion curVersion = new GameVersion(curVersionStr);
        if (oldVersion.IsLower(curVersion))
        {
            deleteUpdateBundle();
        }
    }

    public string getBundleUrl(string fileName)
    {
        #if UNITY_EDITOR
            return Application.dataPath+"/../AssetBundles/" + SysUtil.GetPlatformName() + "/" + fileName;
        #else
            string url = Application.streamingAssetsPath + "/AssetBundles/" + SysUtil.GetPlatformName() + "/" + fileName;
		    if (EnvCheckInit.NeedSyncWithServer)
            {
                string updateDir = Application.persistentDataPath + "/AssetBundles/" + SysUtil.GetPlatformName() + "/" + fileName;
                if (File.Exists(updateDir))
                {
				    url = updateDir;
                }
            }
            return url;
        #endif
    }

    public void deleteUpdateBundle()
    {
        if (Directory.Exists(Application.persistentDataPath + "/AssetBundles"))
        {
            Directory.Delete(Application.persistentDataPath + "/AssetBundles", true);
        }
    }

    private static string getBundleDirName()
    {
#if UNITY_EDITOR
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                return "OSX";
             default:
                return null;
        }
#else
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.OSXPlayer:
                return "OSX";
            default:
                return null;
        }
#endif
    }

    public void LoadAsset(string assetBundleName, string assetName, Action<UnityEngine.Object> fn)
    {
        StartCoroutine(OnLoadAsset(assetBundleName, assetName, fn));
    }

    public IEnumerator OnLoadAsset(string assetBundleName, string assetName, Action<UnityEngine.Object> fn)
    {
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(UnityEngine.Object));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        UnityEngine.Object obj = request.GetAsset<UnityEngine.Object>();
        if(fn!=null) fn(obj);
    }

    public void LoadAllAsset(string assetBundleName, string assetName, Action<UnityEngine.Object[]> fn)
    {
        StartCoroutine(OnLoadAllAsset(assetBundleName, assetName, fn));
    }

    public IEnumerator OnLoadAllAsset(string assetBundleName, string assetName, Action<UnityEngine.Object[]> fn)
    {
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(UnityEngine.Object), false);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        UnityEngine.Object[] obj = request.GetAllAsset<UnityEngine.Object>();
        //Debug.Log(assetName + (obj == null ? " isn't" : " is") + " loaded successfully at frame " + Time.frameCount);
        if(fn!=null) fn(obj);
    }

    public void LoadLevelAsset(string name, Action fn = null)
    {
        string bundle = sceneBundlePath + name;
        StartCoroutine(LoadLevel(bundle.ToLower(), name, fn));
    }

    public void UnloadLevelAsset(string name)
    {
        string bundle = sceneBundlePath + name;
        AssetBundleManager.UnloadAssetBundle(bundle.ToLower(), true);
    }

    private bool is_scene_loaded_ = false;
    public bool IS_SCENE_LOADED
    {
        get
        {
            return is_scene_loaded_;
        }
    }

    private string name_scene_loaded_;
    public string NAME_SCENE_LOADED
    {
        get
        {
            return name_scene_loaded_;
        }
    }

    protected IEnumerator LoadLevel(string assetBundleName, string levelName, Action fn)
    {
        // Debug.Log("Start to load scene " + levelName + " at frame " + Time.frameCount);
        // Load level from assetBundle.
        AssetBundleLoadBaseOperation request = AssetBundleManager.LoadLevelAsync(assetBundleName, levelName, false);
        if (request != null)
        {
            yield return StartCoroutine(request);
        }

        is_scene_loaded_ = true;
        name_scene_loaded_ = levelName;

        if (fn != null)
        {
            fn();
        }
    }

}