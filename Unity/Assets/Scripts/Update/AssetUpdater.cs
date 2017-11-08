using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Junfine.Debuger;
using System;
using System.Threading;
using System.ComponentModel;

/// <summary>
/// ======================== 资源更新 ========================
/// </summary>
public class AssetUpdater : MonoBehaviour
{
	private static AssetUpdater s_instance;
	public static AssetUpdater Instance { get { return s_instance; } }

	const string VERSION_URL = "version.txt";

	const string COMMON_URL  = "Common.zip";

	const string CONTENT_URL = "mark.txt";

	private bool _isDownloading = false;

	public GameVersion srvVersion;

	private EnvCheckInit _envChecker;

	public void Awake()
	{
		s_instance = this;
		_envChecker = GetComponent<EnvCheckInit>();
	}

	public void Update()
	{
		if (_isDownloading)
		{
			lock (AssetDownloader.WebLock)
			{
				if (AssetDownloader.Intance().bFinished)
				{
					_isDownloading = false;
                    _envChecker.downLoadText.text = "";
					_envChecker.LocalVerCheck();
				}
				else
				{
					_envChecker.downloadProcess(AssetDownloader.Intance().Loaded, AssetDownloader.Intance().Total);
				}
			}
		}
	}

	/// <summary>
	/// 获取服务器版本
	/// </summary>
	/// <returns></returns>
	private WWW getVersionWWW()
	{
		return new WWW(ServerConfig.Instance.PatchUrl + "/" + VERSION_URL+ "?" + Time.realtimeSinceStartup.ToString());
	}

	/// <summary>
	/// 获取更新目录
	/// </summary>
	/// <returns></returns>
	private WWW getContentWWW()
	{
		string url = ServerConfig.Instance.PatchUrl + "/";
		url += VersionManager.Instance.getVersionUrl() + "/";
		url += CONTENT_URL;
		SampleDebuger.Log ("getContentURL:  " + url);
		return new WWW(url + "?" + Time.realtimeSinceStartup.ToString());
	}

	/// <summary>
	/// 获取共用资源包
	/// </summary>
	/// <returns></returns>
	private string getCommonURL()
	{
		string url = ServerConfig.Instance.PatchUrl + "/";
		url += VersionManager.Instance.getVersionUrl() + "/";
		url += COMMON_URL;
		return url;
	}

	/// <summary>
	/// 获取本平台资源包
	/// </summary>
	/// <returns></returns>
	private string getCustomURL()
	{
		string url = ServerConfig.Instance.PatchUrl + "/";
		url += VersionManager.Instance.getVersionUrl() + "/";
		url += SysUtil.GetPlatformName() + ".zip"; 
		return url;
	}

	/// <summary>
	/// 检查版本
	/// </summary>
	public void CheckVersionWithServer()
	{
        //*****临时
        EnvCheckInit.NeedSyncWithServer = true;
        if (EnvCheckInit.NeedSyncWithServer)
		{
			StartCoroutine(getServerVersion());
		}
		else
		{
			_envChecker.GameInit();
		}
	}

	IEnumerator getServerVersion()
	{

		WWW www = getVersionWWW();

		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			//无法连接资源服务器
			SampleDebuger.LogWarning ("url " + www.url + " ,error:" + www.error);
			_envChecker.GameInit();
			yield break;
		}

		if (!www.isDone)
			yield return www;

		srvVersion = new GameVersion(www.text.Trim());
		SampleDebuger.Log(" server version = " + srvVersion.ToString());
		VersionManager.Instance.srvVersion = srvVersion.ToString();
		if (VersionManager.Instance.version.IsLower(srvVersion))
		{
			StartCoroutine(checkVersionContent());
			//服务器上有新版本直接大版本更新
		}
		else
		{
			//进入游戏
			_envChecker.GameInit();
		}  
	}

	IEnumerator checkVersionContent(){
		WWW www = getContentWWW();
		SampleDebuger.Log ("+++++++++++ checkVersionContent +++++++++++ ");
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			UpdateTip.Instance.Show();
		}
		//下载更新包
		else
		{
			//UpdateTip.Instance.Show(true, www.text.Trim());
            AssetUpdater.Instance.StartDownloadPatch();
		}


	}

	public void StartDownloadPatch()
	{
		_envChecker.StartDownload();
	}
	public void StartDownload()
	{
		_isDownloading = true;
		AssetDownloader.Intance().AddURL(getCommonURL());
		AssetDownloader.Intance().AddURL(getCustomURL());
		AssetDownloader.Intance().Start();
	}
}

#region 大文件下载器
public class AssetDownloader
{
	private static AssetDownloader _intance = null;
	public static object WebLock = new object();//线程锁
	private WebClient _webClient;
	private Queue<string> _fileQueue = new Queue<string>();
	private string _upzipPath;
	private string _tmpPath;
	//status
	public bool bFinished = false;
	public long Loaded = 0;
	public long Total = 0;

	public int _curFileCount = 0 ;


	public static AssetDownloader Intance()
	{
		if (_intance == null)
		{
			_intance = new AssetDownloader();
		}
		return _intance;
	}

	public void AddURL(string url)
	{
		_fileQueue.Enqueue(url);
	}

	public void init()
	{
		_upzipPath = Application.persistentDataPath;

		_tmpPath = Application.temporaryCachePath;

		_webClient = new WebClient();

		_curFileCount = 0;

		_webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(onDownloadCompelete);

		_webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(onDownloadProcess);

	}

	public void Start()
	{
		bFinished = false;
		init();
		downloadUrl(_fileQueue.Dequeue());
	}

	public void downloadUrl(string url)
	{
        _curFileCount++;

		Uri uri = new Uri(url, UriKind.Absolute);

		string tmpFile = _tmpPath + "/" + _curFileCount + "-" + DateTime.Now.Ticks.ToString();

		_webClient.DownloadFileAsync(uri, tmpFile, tmpFile);

        Debug.Log("@@@++++ downloading url: " + uri.ToString());
    }

	/// <summary>
	/// 下载完成回调
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	void onDownloadCompelete(object sender, AsyncCompletedEventArgs e)
	{
        Debug.Log("@@@ onDownloadCompelete");
		if (e.Error == null && !e.Cancelled)
		{
            FileUtil.DecompressToDirectory(_upzipPath,e.UserState.ToString());
            File.Delete (e.UserState.ToString());

            if (_fileQueue.Count > 0)
			{
                downloadUrl(_fileQueue.Dequeue());
			}
			else
			{
				lock (AssetDownloader.WebLock)
				{
                    bFinished = true;
				}
			}
		}
		else
		{
            _webClient.CancelAsync();
			bFinished = true;
		}

        File.Delete(e.UserState.ToString());

	}

	/// <summary>
	/// 下载进度变化回调
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>

	private void onDownloadProcess(object sender, DownloadProgressChangedEventArgs e)
	{
		Debug.Log(string.Format("@@@received: {0} total: ", e.BytesReceived, e.TotalBytesToReceive));
		lock (AssetDownloader.WebLock)
		{
			Loaded = e.BytesReceived;
			Total = e.TotalBytesToReceive;
        }
	}
}

#endregion