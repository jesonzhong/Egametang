using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

public class BuildVersion
{
    public static bool bLoadAB = false;

    [MenuItem("Mytools/DeleteAllPref ")]
    public static void cleanPlayerPref()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Mytools/Packager ")]
    public static void createWindow()
    {
        BuildWindow win = EditorWindow.GetWindow<BuildWindow>();
        Packager.Init();
        Packager.bLoadAB = !AssetBundleManager.SimulateAssetBundleInEditor;
        Packager.bNeedIPA = Packager.needIPA;
    }

    [MenuItem("Assets/Print Asset Path")]
    private static void PrintSelected()
    {
        foreach (Object obj in Selection.objects)
        {
            if (AssetDatabase.Contains(obj))
            {
                Debug.Log(string.Format("{0} ({1})", AssetDatabase.GetAssetPath(obj), obj.GetType()));
            }
            else
            {
                Debug.LogWarning(string.Format("{0} is not a source asset.", obj));
            }
        }
    }
}

public class BuildWindow : EditorWindow
{

    public string myVersion;

    public string outPutPath = "";


    public string[] verList = new string[3] { "内网测试", "外网测试", "正式版本" };

    public static int curSelect = -1;


    void OnEnable()
    {
        myVersion = VersionManager.Instance.curVersion;
        Packager.Init();
    }

    BuildTargetGroup transPlatform(TargetPlatform plat)
    {
        BuildTargetGroup ret = BuildTargetGroup.Standalone;
        switch (plat)
        {
            case TargetPlatform.IOS:
                ret = BuildTargetGroup.iOS;
                break;
            case TargetPlatform.Android:
                ret = BuildTargetGroup.Android;
                break;

        }
        return ret;
    }

    void OnGUI()
    {

        GUILayout.Label(" 指定当前版本号, 与服务器保持一致", EditorStyles.boldLabel);
        myVersion = GUILayout.TextField(myVersion);
        GUILayout.Space(20);
        if (myVersion != VersionManager.Instance.curVersion)
            VersionManager.Instance.curVersion = myVersion;

        PlayerSettings.bundleVersion = myVersion;
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            PlayerSettings.Android.bundleVersionCode = VersionManager.Instance.getVersionNum();
        }
        GUILayout.Space(30);

        //=========================== 1.选择平台 ================================
        GUILayout.Label(" 选择发布平台  ", EditorStyles.boldLabel);
        GUILayout.Space(20);
        TargetPlatform select = (TargetPlatform)EditorGUILayout.EnumPopup(Packager.curTarget);

        if (select != Packager.curTarget)
        {
            // 重新判断当前版本设定
            Packager.curTarget = select;
           
        }

        GUILayout.Space(20);

        // =========================== 3. 标记AB资源   ===========================
        if (GUILayout.Button("标记AB", GUILayout.Height(30)))
        {
            Packager.BuildAssetMarks();
            Packager.WritePreloadFile();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("生成AB", GUILayout.Height(30)))
        {
            Packager.ClearABFolder();
            //Packager.BuildAssetMarks();
			Packager.GenerateAB();
        }

        // =========================== 4. 是否读取AB包  ===========================
        bool cur = GUILayout.Toggle(Packager.bLoadAB, "读取AB包");

        if (Packager.bLoadAB != cur)
        {
            Packager.bLoadAB = cur;
            AssetBundleManager.SimulateAssetBundleInEditor = !Packager.bLoadAB;
        }

        GUILayout.Space(20);
        GUIContent content = new GUIContent(" 请确认完成了 AB包 的制做过程 ！！！");
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Normal;
        style.fontSize = 13;

        GUILayout.Label(content);
        GUILayout.Space(20);
        GUILayout.Label(" 选择发布版本类型:");
        GUILayout.Space(20);
        BuildTargetGroup curGroup = transPlatform(Packager.curTarget);
        string curSymbol = null;
        if (curSelect == -1)
        {
            curSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(curGroup);
            if (curSymbol.IndexOf("RELEASE_VER", 0, curSymbol.Length) == -1){
                curSelect = 0;
                PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";
            }
            else
            {
                if (curSymbol.IndexOf("STORE_VERSION", 0, curSymbol.Length) == -1)
                {
                    curSelect = 1;
                    PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";
                }
                else
                {
                    curSelect = 2; 
                    PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";
                }
            }

        }

        int newSelect = GUILayout.SelectionGrid(curSelect, verList, 6);

        //处理不同版本的一些 PlayerSetting 设置
        if (newSelect != curSelect)
        {
            curSelect = newSelect;
            int offset;
            switch (curSelect)
            {
                case 0:
                    curSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(curGroup);
					curSymbol = curSymbol.Replace(";RELEASE_VER", "");
                    curSymbol = curSymbol.Replace("RELEASE_VER", "");
                    curSymbol = curSymbol.Replace(";STORE_VERSION", "");
                    curSymbol = curSymbol.Replace("STORE_VERSION", "");
                    PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";
                    break;

                case 1:
                    curSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(curGroup);
					offset = curSymbol.IndexOf(";RELEASE_VER", 0, curSymbol.Length);
                    curSymbol = curSymbol.Replace(";STORE_VERSION", "");
                    if (offset == -1)
						curSymbol = curSymbol + ";RELEASE_VER";
                    PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";
                    break;

                case 2:
                    curSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(curGroup);
					offset = curSymbol.IndexOf(";RELEASE_VER", 0, curSymbol.Length);
                    if (offset == -1)
						curSymbol = curSymbol + ";RELEASE_VER";

                    offset = curSymbol.IndexOf(";STORE_VERSION", 0, curSymbol.Length);
                    if (offset == -1)
                        curSymbol = curSymbol + ";STORE_VERSION";
                    PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";
                    break;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(curGroup, curSymbol);
            Debug.Log(curSymbol);
        }

        GUILayout.Space(20);
        // =========================== 5. 生成安装包    ===========================
        if (GUILayout.Button("生成安装包 ", GUILayout.Height(30)))
        {
            if (myVersion.Length == 0 || myVersion.Equals("0.0.0"))
            {
                EditorUtility.DisplayDialog(" Error ！！", " 请修改版本为有效数字", "确定");
            }
            else
            {
                switch (Packager.curTarget)
                {
                    case TargetPlatform.IOS:
                        BuildUtil.buildIOS();
                        //清理并生成指纹数据
                        BuildUtility.CleanAndGenFileList();
                            
                        break;
                    case TargetPlatform.Window:

                        BuildUtil.buildWindows();
                        break;
                    case TargetPlatform.Android:
                        BuildUtil.buildAndroid();
                        BuildUtility.CleanAndGenFileList();

                        break;
                }
            }
        };
        GUILayout.Space(20);

        if (Packager.curTarget == TargetPlatform.IOS)
        {

            if (GUILayout.Button("生成IPA", GUILayout.Height(30)))
            {
                IPABuilder.buildIPA();

            }
            GUILayout.Space(20);

        }

        if (GUILayout.Button("生成版本更新包 ", GUILayout.Height(30)))
        {
			BuildUtil.copyFullABRes ();
            PatchUtil pu = PatchUtil.Instance;
            pu.init();
            pu.buildPatch();
        }
    }
}

public class BuildUtil
{
    static string[] levels = { "Assets/Scenes/launcher.unity" };

    static public string getPath() 
    {
        int tmp = Application.dataPath.LastIndexOf("/");
        string path = Application.dataPath.Substring(0, tmp);
        return path;
    }

    static public void buildIOS()
    {
        BuildTarget type = BuildTarget.iOS;
        copyABRes(type);
        createVersion();
        AssetDatabase.Refresh();
        BuildPipeline.BuildPlayer(levels, BuildUtil.getPath() + "/proj_ios", BuildTarget.iOS, BuildOptions.Il2CPP | BuildOptions.ShowBuiltPlayer);
    }

    static public void buildAndroid()
    {
        BuildTarget type = BuildTarget.Android;
        copyABRes(type);
        createVersion();
        AssetDatabase.Refresh();
        BuildPipeline.BuildPlayer(levels, BuildUtil.getPath() + "/sxzs2017.apk", BuildTarget.Android, BuildOptions.ShowBuiltPlayer);

    }

    static public void buildWindows()
    {
        BuildTarget type = BuildTarget.StandaloneWindows;
        copyABRes(type);
        createVersion();
        AssetDatabase.Refresh();
        BuildPipeline.BuildPlayer(levels, BuildUtil.getPath() + "/proj_win/sxzs2017.exe", BuildTarget.StandaloneWindows, BuildOptions.ShowBuiltPlayer | BuildOptions.Development);
    }

    static public void cleanABPath()
    {
        deleteDirectroy(Application.streamingAssetsPath + "/AssetBundles");
    }

    static public void copyABRes(BuildTarget os)
    {
        cleanABPath();
        string osPath = getPlatformDir(os);
        _copyDirectory(Application.dataPath + "/../AssetBundles/" + osPath, 
            Application.streamingAssetsPath + "/AssetBundles/" + osPath,
            new string[] { ".manifest", ".meta"},
            new string[] { osPath + ".manifest" });
    }

	static public void copyFullABRes()
	{
		cleanABPath();
		copyDirectory(Application.dataPath + "/../AssetBundles",  Application.streamingAssetsPath + "/AssetBundles");
	}


    static public string getPlatformDir(BuildTarget os){
        switch (os)
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
    }

    static public string getPlatformManifest()
    {
        return getPlatformDir(Packager.getBuildTarget()) + ".manifest";

    }


    static public void deleteDirectroy(string dirName)
    {
        DirectoryInfo d = new DirectoryInfo(dirName);
        if (d.Exists)
        {
            Directory.Delete(dirName, true);
        }
    }

    static public void copyDirectory(string fromDir, string toDir)
    {
        _copyDirectory(fromDir, toDir);
    }

    static public void _copyDirectory(string fromDir, string toDir, string[] ignoreExts = null, string[] needFiles = null)
    {
        if (Directory.Exists(fromDir))
        {
            if (!Directory.Exists(toDir))
            {
                Directory.CreateDirectory(toDir);
            }
            string[] files = Directory.GetFiles(fromDir, "*", SearchOption.AllDirectories);
            string[] dirs = Directory.GetDirectories(fromDir, "*", SearchOption.AllDirectories);
            foreach (string soureDir in dirs)
            {
                string desDir = soureDir.Replace(fromDir, toDir);
                Debug.Log("path: " + desDir);
                if (!Directory.Exists(desDir))
                {
                    Directory.CreateDirectory(desDir);
                }
            }

            foreach (string soureFile in files)
            {
                string extName = Path.GetExtension(soureFile);
                string fileName = Path.GetFileName(soureFile);
                if (needFiles != null && needFiles.Contains<string>(fileName))
                {
                    File.Copy(soureFile, soureFile.Replace(fromDir, toDir), true);

                }
                else if (!string.IsNullOrEmpty(extName) && ignoreExts != null && ignoreExts.Contains<string>(extName))
                {
                    Debug.Log("ignoreFile: " + soureFile);

                }
                else
                {
                    File.Copy(soureFile, soureFile.Replace(fromDir, toDir), true);
                }
            }
        }
    }

    static public void createVersion()
    {
        string streamPath = Application.streamingAssetsPath;

        FileInfo fi = new FileInfo(streamPath + "/streamPath.txt");
        using (StreamWriter sw = fi.CreateText())
        {
            getFilePath(streamPath, sw);
        }

        AssetDatabase.Refresh();
        Debug.Log("新版本生成成功,  ");
    }

    static public void getFilePath(string sourcePath, StreamWriter sw)
    {

        DirectoryInfo info = new DirectoryInfo(sourcePath);

        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {

            if (fsi.Extension != ".meta" && fsi.Name != "streamPath.txt")
            {

                string[] r = fsi.FullName.Split(new string[] { "StreamingAssets" }, System.StringSplitOptions.None); //得到相对路径

                r[1] = r[1].Replace('\\', '/'); //安卓上只能识别"/"

                if (fsi is DirectoryInfo)
                { //是文件夹则迭代

                    sw.WriteLine(r[1] + " | 0"); //按行写入
                    bool ignored = fsi.FullName.EndsWith("AssetBundles");
                    if (!ignored)
                    {
                        getFilePath(fsi.FullName, sw);
                    }

                }
                else
                {
                    sw.WriteLine(r[1] + " | 1" + "|" + string.Format("{0:F}", ((FileInfo)fsi).Length / 1024.0f)); //按行写入
                }
            }
        }
    }

    static public string getVersionNum()
    {
        string streamPath = Application.streamingAssetsPath;
        byte[] ret = File.ReadAllBytes(streamPath + "/version.txt");
        if (ret == null || ret.Length == 0)
            return "1.0.0";
        string versionNum = System.Text.Encoding.Default.GetString(ret);
        return versionNum;
    }
}

/// <summary>
/// 补丁生成工具
/// </summary>
public class PatchUtil
{
    static PatchUtil _instance = null;
    string sourcePath = Application.streamingAssetsPath;//源文件路径
    //补丁存放路径
    public string targetPath
    {
        get 
        {
#if UNITY_IOS
            return  Application.dataPath + "/../patch/iOS/";
#elif UNITY_ANDROID
            return  Application.dataPath + "/../patch/Android/";
#else 
            return  Application.dataPath + "/../patch/Windows/";
#endif
            
        }
    }

    List<PatchInfo> patchList = new List<PatchInfo>();

    public static PatchUtil Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PatchUtil();
                _instance.init();
            }
            return _instance;
        }
    }

    public void init()
    {
        patchList.Clear();
        updatePatchList();
    }

    void updatePatchList()
    {
        string[] foldList = Directory.GetDirectories(targetPath);
        for (int i = 0; i < foldList.Length; i++)
        {
            string folderName = foldList[i];
            string vername = Path.GetFileName(folderName).Replace("_", ".");
            PatchInfo info = new PatchInfo(vername);
            info.loadContent(folderName);
            if (info.isVaild)
            {
                patchList.Add(info);
            }
        }
    }


	public void buildPatch()
	{
		//1. 创建当前版本目录
		string folderName = VersionManager.Instance.curVersion.Replace(".", "_");
		folderName = targetPath + folderName;
		if (Directory.Exists(folderName)) return;

		PatchInfo patchInfo = new PatchInfo(VersionManager.Instance.curVersion);
		Directory.CreateDirectory(folderName);

		//2. 统计当前版本所有文件信息，保存至文本文件
		List<string> fileSystemEntries = new List<string>();

		fileSystemEntries.AddRange(Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories));

		FileStream fs = new FileStream(folderName + "/files.txt", FileMode.CreateNew);
		StreamWriter sw = new StreamWriter(fs);
		for (int i = 0; i < fileSystemEntries.Count; i++)
		{
			string file = fileSystemEntries[i];
			file = file.Replace("\\", "/");
			string ext = Path.GetExtension(file);

			if (file.EndsWith(".meta") || file.Contains(".DS_Store") || file.Contains("Audio/GeneratedSoundBanks") || (file.Contains(".manifest")&&!(file.Contains(BuildUtil.getPlatformManifest()) ))) continue;

			FileStream fileStream = new FileStream(file, FileMode.Open);

			int size = (int)fileStream.Length;
            string md5 = "";
            //是场景并且对应.manifest文件存在
            if ((file.Contains("assets\\bundles\\scene") || file.Contains("assets/bundles/scene")) && (File.Exists(file + ".manifest")))
            {
                md5 = FileUtil.FetchMD5(file + ".manifest");
            }
            else
            {
                md5 = FileUtil.FSToMD5(fileStream);
            }

			string value = file.Replace(sourcePath, string.Empty).Substring(1);
			string content = value + "|" + md5 + "|" + size;
			UpdateFileInfo fileInfo = new UpdateFileInfo(content);
			patchInfo.addFileInfo(content);
			sw.WriteLine(content);
			fileStream.Close();
			Packager.UpdateProgress(i, fileSystemEntries.Count, "Generating file list..");
		}
		sw.Close(); fs.Close();

		//3.与历史版本对比压缩所有差异文件
		foreach (PatchInfo pInfo in patchList)
		{
			ArrayList diffFiles = pInfo.getDiffFiles(patchInfo);
			if (diffFiles.Count == 0) continue;

			FileStream commonStream = new FileStream(pInfo.getPatchPath() + "/Common.zip", FileMode.Create);
			ZipOutputStream commonZipper = new ZipOutputStream(commonStream);
			commonZipper.SetLevel(5);

			FileStream iosStream = new FileStream(pInfo.getPatchPath() + "/iOS.zip", FileMode.Create);
			ZipOutputStream iosZipper = new ZipOutputStream(iosStream);
			iosZipper.SetLevel(5);

			ZipOutputStream androidZipper = new ZipOutputStream(new FileStream(pInfo.getPatchPath() + "/Android.zip", FileMode.Create));
			androidZipper.SetLevel(5);
			ZipOutputStream winZipper = new ZipOutputStream(new FileStream(pInfo.getPatchPath() + "/Windows.zip", FileMode.Create));
			winZipper.SetLevel(5);

			string versionNum = pInfo.ver.curVersion;
			for (int i = 0; i < diffFiles.Count; i++)
			{
				string fileName = diffFiles[i] as string;
				ZipOutputStream compressor = commonZipper;
				if (fileName.Contains("AssetBundles/iOS/"))
				{
					compressor = iosZipper;
				}
				else if(fileName.Contains("AssetBundles/Windows/"))
				{
					compressor = winZipper;
				}
				else if(fileName.Contains("AssetBundles/Android/"))
				{
					compressor = androidZipper;
				}
				compressor.PutNextEntry(new ZipEntry(fileName));
				string fullPath = sourcePath + "/" + fileName;
				Packager.UpdateProgress(i, diffFiles.Count, " Compress version: " + versionNum + " on file: " + fileName);
				byte[] data = new byte[2048];
				using (FileStream input = File.OpenRead(fullPath))
				{
					int bytesRead;
					while ((bytesRead = input.Read(data, 0, data.Length)) > 0)
					{
						compressor.Write(data, 0, bytesRead);
					}
				}
			}
			commonZipper.Finish();
			iosZipper.Finish();
			androidZipper.Finish();
			winZipper.Finish();

			FileStream markFs = new FileStream(pInfo.getPatchPath() + "/mark.txt", FileMode.Truncate);
			StreamWriter markSw = new StreamWriter(markFs);

            long patchSize = iosZipper.Length + commonZipper.Length;
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                patchSize = androidZipper.Length + commonZipper.Length;

			int splitSize = 1024 * 1024;
			float result = (float)patchSize / (float)splitSize;
			markSw.Write(result.ToString("0.000") + "MB");
			markSw.Close();
			markFs.Close();


		}

		//4. 记录当前版本号
		VersionManager.Instance.saveVersion(targetPath + "version.txt");


		fs = new FileStream(folderName + "/mark.txt", FileMode.CreateNew);
		StreamWriter tsw = new StreamWriter(fs);
		tsw.Write("0");
		tsw.Close();
		fs.Close ();
		EditorUtility.ClearProgressBar();
	}
}

/// <summary>
/// 补丁包信息
/// </summary>
public class PatchInfo
{
    public Dictionary<string, UpdateFileInfo> fileList = new Dictionary<string, UpdateFileInfo>();
    public GameVersion ver = null;
    public bool isVaild = false;
    public string storePath = "";

    public PatchInfo(string name)
    {
        ver = new GameVersion(name);
    }

    //从文本文件读取信息
    public void loadContent(string path)
    {
        fileList.Clear();
        string ret = null;
        path += "/files.txt";
        try
        {
            ret = File.ReadAllText(path);
            string[] fileContent = ret.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < fileContent.Length; i++)
            {
                UpdateFileInfo fileInfo = new UpdateFileInfo(fileContent[i]);
                fileList[fileInfo.filePath] = fileInfo;
            }
            isVaild = true;
        }
        catch
        {
            Debug.Log(" 没有找到文件 files.txt");
        }
        storePath = Path.GetDirectoryName(path);
    }

    //添加文件信息
    public void addFileInfo(string content)
    {
        UpdateFileInfo info = new UpdateFileInfo(content);
        fileList[info.filePath] = info;
    }

    //获取差异文件
    public ArrayList getDiffFiles(PatchInfo other)
    {
        ArrayList list = new ArrayList();
        foreach (KeyValuePair<string, UpdateFileInfo> info in other.fileList)
        {
            string key = info.Key;
            UpdateFileInfo newInfo = info.Value;
            UpdateFileInfo curInfo = null;
            fileList.TryGetValue(key, out curInfo);
            if (curInfo != null && curInfo.equal(newInfo))
                continue;
            list.Add(key);
        }
        return list;
    }

    //获取补丁路径
    public string getPatchPath()
    {
        return storePath;
    }
}

public class BuildUtility
{

    public static void SetVersion()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        if (args == null)
        {
            return;
        }
        int index = 0;
        for (; index < args.Length; index++)
        {
            if (args[index] == "BuildUtility.SetVersion")
            {

                break;
            }
        }
        if (index < args.Length - 1)
        {
            VersionManager.Instance.curVersion = args[index + 1];
            PlayerSettings.bundleVersion = VersionManager.Instance.curVersion;
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                PlayerSettings.Android.bundleVersionCode = VersionManager.Instance.getVersionNum();
            }
        }
        else
        {
            Debug.LogError("version argument error");
        }
    }


    public static void OnCreateIOSAssetBundle()
    {
        Debug.Log("start OnCreateIOSAssetBundle");

        Packager.curTarget = TargetPlatform.IOS;

        //标记
        Packager.BuildAssetMarks();
        Packager.WritePreloadFile();

        //生成
        Packager.ClearABFolder();
        Packager.GenerateAB();

        Debug.Log("end OnCreateIOSAssetBundle");
    }

    public static void OnCreateAndroidAssetBundle()
    {
        Debug.Log("start OnCreateAndroidAssetBundle");

        Packager.curTarget = TargetPlatform.Android;
        
        //标记
        Packager.BuildAssetMarks();
        Packager.WritePreloadFile();
        
        //生成
        Packager.ClearABFolder();
        Packager.GenerateAB();

        Debug.Log("end OnCreateAndroidAssetBundle");
    }

	//内网
    public static void OnGenXCodeProjectLocal()
    {
		Debug.Log("start OnGenXCodeProjectLocal");

        string curSymbol = null;

        curSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        curSymbol = curSymbol.Replace(";RELEASE_VER", "");
        curSymbol = curSymbol.Replace(";STORE_VERSION", "");
        PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, curSymbol);
        Debug.Log(curSymbol);

        BuildUtil.buildIOS();

        //清理并生成指纹数据
        BuildUtility.CleanAndGenFileList();

		Debug.Log("end OnGenXCodeProjectLocal");
    }

	//外网测试
	public static void OnGenXCodeProjectBeta()
	{
		Debug.Log("start OnGenXCodeProjectBeta");

		string curSymbol = null;

		curSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
		int offset = curSymbol.IndexOf(";RELEASE_VER", 0, curSymbol.Length);
		curSymbol = curSymbol.Replace(";STORE_VERSION", "");
		if (offset == -1)
			curSymbol = curSymbol + ";RELEASE_VER";
		PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";

		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, curSymbol);
		Debug.Log(curSymbol);

		BuildUtil.buildIOS();

        //清理并生成指纹数据
        BuildUtility.CleanAndGenFileList();

		Debug.Log("end OnGenXCodeProjectBeta");
	}

    //清理并生成指纹数据
    public static void CleanAndGenFileList()
    {
        PatchUtil pu = PatchUtil.Instance;
        string[] foldList = Directory.GetDirectories(pu.targetPath);
        for (int i = 0; i < foldList.Length; i++)
        {
            string folderName = foldList[i];
            BuildUtil.deleteDirectroy(folderName);
        }
        pu.init();
        pu.buildPatch();
    }

	//app store
	public static void OnGenXCodeProjectStore()
	{
		Debug.Log("start OnGenXCodeProjectStore");

		string curSymbol = null;

		curSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
		int offset = curSymbol.IndexOf(";RELEASE_VER", 0, curSymbol.Length);
		if (offset == -1)
			curSymbol = curSymbol + ";RELEASE_VER";

		offset = curSymbol.IndexOf(";STORE_VERSION", 0, curSymbol.Length);
		if (offset == -1)
			curSymbol = curSymbol + ";STORE_VERSION";
		PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";

		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, curSymbol);
		Debug.Log(curSymbol);

		BuildUtil.buildIOS();

		Debug.Log("end OnGenXCodeProjectStore");
	}

	//内网
    public static void OnBuildAndroidLocal()
    {
		Debug.Log("start OnBuildAndroidLocal");

		string curSymbol = null;

		curSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
		curSymbol = curSymbol.Replace(";RELEASE_VER", "");
		curSymbol = curSymbol.Replace(";STORE_VERSION", "");
		PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";

		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, curSymbol);
		Debug.Log(curSymbol);

        BuildUtil.buildAndroid();
        //清理并生成指纹数据
        BuildUtility.CleanAndGenFileList();

		Debug.Log("end OnBuildAndroidLocal");
    }

	//外网测试
	public static void OnBuildAndroidBeta()
	{
		Debug.Log("start OnBuildAndroidBeta");

		string curSymbol = null;

		curSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
		int offset = curSymbol.IndexOf(";RELEASE_VER", 0, curSymbol.Length);
		curSymbol = curSymbol.Replace(";STORE_VERSION", "");
		if (offset == -1)
			curSymbol = curSymbol + ";RELEASE_VER";
		PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";

		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, curSymbol);
		Debug.Log(curSymbol);

		BuildUtil.buildAndroid();
        //清理并生成指纹数据
        BuildUtility.CleanAndGenFileList();

		Debug.Log("end OnBuildAndroidBeta");
	}

	//渠道包
	public static void OnBuildAndroidStore()
	{
		Debug.Log("start OnBuildAndroidStore");

		string curSymbol = null;

		curSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
		int offset = curSymbol.IndexOf(";RELEASE_VER", 0, curSymbol.Length);
		if (offset == -1)
			curSymbol = curSymbol + ";RELEASE_VER";

		offset = curSymbol.IndexOf(";STORE_VERSION", 0, curSymbol.Length);
		if (offset == -1)
			curSymbol = curSymbol + ";STORE_VERSION";
		PlayerSettings.applicationIdentifier = "com.ztgame.sxzs";

		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, curSymbol);
		Debug.Log(curSymbol);

		BuildUtil.buildAndroid();

		Debug.Log("end OnBuildAndroidStore");
	}

    public static void CreatePatch()
    {
        BuildUtil.copyFullABRes();
        PatchUtil pu = PatchUtil.Instance;
        pu.init();
        pu.buildPatch();
    }


    public static void OnBuildIPA()
    {
        IPABuilder.buildIPA();
    }

}