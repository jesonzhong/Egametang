using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
public class EnvCheckInit : UEventEmitter
{
    private AssetUpdater _assetUpdater;

    public Text statusText;

    public Text descText;

    public Text downLoadText;
    public Slider progressSlider;

    public Image bottomImage;

#if UNITY_EDITOR || !RELEASE_VER
    static public bool NeedSyncWithLocal = true;
    static public bool NeedSyncWithServer = false;

#elif UNITY_STANDALONE_WIN
    static public bool NeedSyncWithLocal = true;
    static public bool NeedSyncWithServer = false;
#else
    static public bool NeedSyncWithLocal = true;
    static public bool NeedSyncWithServer = true;
#endif

    string currentVersion;
    string serverVersionText;
    string localVersionText;
    string downLoadingResText;
    string gameInitText;
    string loadResText;

    void Awake()
    {
#if RELEASE_VER
        EnvCheckInit.NeedSyncWithServer = MassiveConfig.Instance.NeedSyncWithServer;
#endif

        //调试版不检测更新
        if (UnityEngine.Debug.isDebugBuild == true)
        {
            EnvCheckInit.NeedSyncWithServer = false;
        }

        // 启动服务器同步检查模块
        _assetUpdater = GetComponent<AssetUpdater>();
        progressSlider.gameObject.SetActive(false);
        GClientConfig.Load();

#if UNITY_EDITOR
        Application.targetFrameRate = 60;
        QualitySettings.SetQualityLevel(5);
        QualitySettings.vSyncCount = 1;
#else
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        QualitySettings.SetQualityLevel(5);
#endif

        QualitySettings.shadowResolution = ShadowResolution.High;
        QualitySettings.shadowDistance = 1000;
        QualitySettings.shadowCascades = 2;
        QualitySettings.shadowProjection = ShadowProjection.StableFit;
      

        //serverVersionText = LanguageMgr.Instance.GetServerVersionText();
        //localVersionText = LanguageMgr.Instance.GetLocalVersionText();
        //downLoadingResText = LanguageMgr.Instance.GetDownLoadingResText();
        //gameInitText = LanguageMgr.Instance.GetGameInitText();
        //loadResText = LanguageMgr.Instance.GetLoadResText();
        //descText.text = LanguageMgr.Instance.GetLoadResText();
        //LanguageMgr.Instance.SetBottomImageVisible(bottomImage);
        Once("LuaMangager_Started", gameStart);
        //Once("MainPlayer_DataLoaded", dataLoaded);
        On("MainPlayer_DataLoaded_All", dataLoaded);
    }

    void Start()
    {
        //预加载通用的ab包
        progressSlider.gameObject.SetActive(false);
        //通知加载完毕
        LocalVerCheck();
    }

    private void gameStart(params object[] arg)
    {
        AssetManager.Instance.PreloadAsset((done, total) =>
        {
            copyProcess(done, total, loadResText);
            if (done >= total)
            {
                //预加载完成跳到登陆界面
                AssetBundleLoader.Instance.LoadLevelAsset("login", () =>
                {
                    AssetBundleManager.UnloadAssetBundle("assets/bundles/scene/updater", true);

                    //UGuiManager.Initialize();
                    //GiantMobileManager.Instance.Init();
#if UNITY_EDITOR
                     //LoginManager.Instance.OnInitLogin();
#else                  


#endif
                });


            }
        });
    }

    /// <summary>
    /// 本地版本检查
    /// </summary>
    public void LocalVerCheck()
    {
        if (EnvCheckInit.NeedSyncWithLocal){
            progressSlider.gameObject.SetActive(true);
            StartCoroutine(checkVersion());
        }
        else{
            GameInit();
        }
    }

    /// <summary>
    /// 版本检查
    /// </summary>
    /// <returns></returns>
    IEnumerator checkVersion()
    {
        string path = Globals.wwwPersistenPath + "/version.txt";
        WWW www = new WWW(path);
        yield return www;
        SampleDebuger.Log("version  = " + www.text);

        if (string.IsNullOrEmpty(www.text))
        { //没读取到，是第一次安装，拷贝资源
            SampleDebuger.Log("First Time Launch!");
            //读取应用程序版本号
            www = new WWW(Globals.wwwStreamingPath + "/version.txt");
            yield return www;
            currentVersion = www.text.Trim();
            beginCopy();
            www.Dispose();
        }
        else
        { //已安装过
            SampleDebuger.Log(" installed");
            string oldVersion = www.text.Trim();       //读取当前旧版本号

            //读取应用程序版本号
            www = new WWW(Globals.wwwStreamingPath + "/version.txt");
            yield return www;
            currentVersion = www.text.Trim();

            //版本号小于安装程序中包含的版本号，删除旧资源再拷贝当前资源

            GameVersion old_v = new GameVersion(oldVersion);
            GameVersion app_v = new GameVersion(currentVersion);
            if (old_v.IsLower(app_v))
            {
                string abPath = Application.persistentDataPath + "/AssetBundles";
                FileUtil.RemoveFolder(abPath);
                beginCopy();
            }
            else
            {
                VersionManager.Instance.curVersion = oldVersion;
                _assetUpdater.CheckVersionWithServer();
            }
        }
    }


    /// <summary>
    /// 开始下载
    /// </summary>
    public void StartDownload()
    {
        statusText.text = serverVersionText + _assetUpdater.srvVersion.ToString() +
                "\n" + localVersionText + VersionManager.Instance.version.ToString();
        statusText.gameObject.SetActive(true);
        descText.text = "正在下载";
        progressSlider.value = 0;
        _assetUpdater.StartDownload();
    }


    /// <summary>
    /// 更新下载进度
    /// </summary>
    /// <param name="done"></param>
    /// <param name="total"></param>
    public void downloadProcess(long done, long total)
    {
        Debug.Log("++ done: " + done + " total: " + total);
        progressSlider.value = done > 0 ? (float)done / (float)total : 0;
        statusText.text = "";

        downLoadText.text = downLoadingResText + "(" + (done / 1048576).ToString("F2") + "MB /" + (total / 1048576).ToString("F2") + "MB) (" + ((int)(progressSlider.value * 100)).ToString() + "% )"; ;

    }


    /// <summary>
    /// 更新拷贝进度
    /// </summary>
    /// <param name="done"></param>
    /// <param name="total"></param>
    /// <param name="content"></param>
    public void copyProcess(int done, int total, string content)
    {
        float percent = done > 0.0f ? (float)(done+1) / (float)total : 0.0f;
        if (percent > 1.0f)
            percent = 1.0f;
        progressSlider.value = percent;
        statusText.text = ((int)(percent * 100)).ToString() + "%";
        descText.text = content; //"游戏初始化中，此过程不消耗流量！ ";
    }

    ///<summary>
    ///开始拷贝, 完成更新应用程序的同步过程
    ///即解压安装过程
    /// </summary>
    private void beginCopy()
    {
        StartCoroutine(_beginCopy(Globals.wwwStreamingPath));
    }

    /// <summary>
    /// 将文件拷贝到路径中 
    /// </summary>
    /// <param name="streamPath"></param>
    /// <returns></returns>
    IEnumerator _beginCopy(string path)
    {
        WWW www = new WWW(path + "/streamPath.txt");
        yield return www;

        string[] content = www.text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        www.Dispose();

        int total = content.Length;
        int curIndex = 0;
        float timeOut = 5.0f;
        foreach (string item in content)
        {
            string it = item.Trim(); //window下会有\r，需要删除
            int fileFlag = int.Parse(it.Split('|')[1]);
            it = it.Split('|')[0];
            SampleDebuger.Log(path);
            it = it.Trim();

            if (fileFlag == 1)
            {
                //第一次
                www = new WWW(path + it);
                //yield return www;

                float timer = 0;
                bool failed = false;
                while (!www.isDone)
                {
                    if (timer > timeOut) { failed = true; break; }

                    timer += Time.deltaTime;

                    yield return null;
                }

                if (!failed)
                {
                    File.WriteAllBytes(Application.persistentDataPath + it, www.bytes);
                    //更新进度
                    copyProcess(curIndex, total, gameInitText);
                }
                else
                {
                    //FloatTextMgr.Instance.NewText("Copy failed: " + path + it, 20, Color.red);
                    Debug.LogError("@@@Copy failed: " + path + it);
                    //第二次
                    WWW www2 = new WWW(path + it);

                    float timer2 = 0;
                    bool failed2 = false;
                    while (!www2.isDone)
                    {
                        if (timer2 > timeOut) { failed2 = true; break; }

                        timer2 += Time.deltaTime;

                        yield return null;
                    }

                    if (!failed2)
                    {
                        File.WriteAllBytes(Application.persistentDataPath + it, www2.bytes);
                        //更新进度
                        copyProcess(curIndex, total, gameInitText);

                        www2.Dispose();
                    }
                    else
                    {
                        //FloatTextMgr.Instance.NewText("Copy failed2: " + path + it, 20, Color.red);
                        Debug.LogError("@@@Copy failed2: " + path + it);

                        //第三次
                        WWW www3 = new WWW(path + it);

                        float timer3 = 0;
                        bool failed3 = false;
                        while (!www3.isDone)
                        {
                            if (timer3 > timeOut) { failed3 = true; break; }

                            timer3 += Time.deltaTime;

                            yield return null;
                        }

                        if (!failed3)
                        {
                            File.WriteAllBytes(Application.persistentDataPath + it, www3.bytes);
                            //更新进度
                            copyProcess(curIndex, total, gameInitText);

                            www3.Dispose();
                        }
                        else
                        {
                            Debug.LogError("@@@Copy failed3: " + path + it);
                            //FloatTextMgr.Instance.NewText("Copy failed3: " + path + it, 20, Color.red);
                        }
                    }
                }
                www.Dispose();

            }
            else if (fileFlag == 0)
            {
                SampleDebuger.Log("Create dir " + Application.persistentDataPath + it);
                Directory.CreateDirectory(Application.persistentDataPath + it);
            }
            else
            {
                SampleDebuger.LogError("既不是文件夹也不是文件 路径为");
            }
            ++curIndex;
        }

        SampleDebuger.Log(" writeversion");

        // 同步版本
        VersionManager.Instance.curVersion = currentVersion;
        _assetUpdater.CheckVersionWithServer();
    }


    public void GameInit()
    {
        AssetBundleLoader.Instance.LoadLevelAsset("main", () =>
        {
            //AssetBundleManager.UnloadAssetBundle("assets/bundles/scene/login", true);
        });
    }

    public void SDKInit()
    {

    }

    public void EnterLogin(params object[] args)
    {
		GameInit ();
    }

    private void dataLoaded(params object[] args)
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "login")
        {
            AssetBundleLoader.Instance.LoadLevelAsset("main", () =>
            {
                //AssetBundleManager.UnloadAssetBundle("assets/bundles/scene/login", true);
            });
        }
    }

    public void BackClickEvent()
    {
        EnterLogin();
    }
}
