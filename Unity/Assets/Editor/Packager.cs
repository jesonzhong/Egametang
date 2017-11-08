using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public enum TargetPlatform{
    IOS,
    Window,
    Android,
    inValid,
}

public class Packager
{
    public static string platform = string.Empty;

    public static bool bLoadAB = false;

    public static bool bNeedIPA = false;

    private static bool bEncodeLua = false;

    private static bool bAssetBundle = true;

    public static TargetPlatform curTarget = TargetPlatform.IOS;

    public static void Init() 
    {
        // 判断当前所在平台 
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            Packager.curTarget = TargetPlatform.Android;
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            Packager.curTarget = TargetPlatform.IOS;
        }
        else
            Packager.curTarget = TargetPlatform.Window;

    }

    public static bool needIPA
    {
        get
        {
            Packager.bNeedIPA = EditorPrefs.GetBool("needGenrateIPA", true);
            return Packager.bNeedIPA;
        }
        set
        {
            if (value != Packager.bNeedIPA)
            {
                Packager.bNeedIPA = value;
                EditorPrefs.SetBool("needGenrateIPA", value);
            }
        }
    }


    public static BuildTarget getBuildTarget()
    {
        BuildTarget target = BuildTarget.StandaloneWindows;
        switch (curTarget)
        {
            case TargetPlatform.IOS:
                target = BuildTarget.iOS;
                break;
            case TargetPlatform.Android:
                target = BuildTarget.Android;
                break;

            case TargetPlatform.Window:
                target = BuildTarget.StandaloneWindows;
                break;
        }
        return target;
    }

    /// <summary>
    /// 生成绑定素材
    /// </summary>
    public static void BuildAssetResource()
    {
        //1.生成AB包
        Packager.GenerateAB();
    }

	public static string GetABPath(){
		return Application.dataPath + "/../AssetBundles/" + Packager.GetPlatformFolderForAssetBundles(Packager.getBuildTarget());
	}

    //生成AB包
    public static void GenerateAB()
    {
		string resPath = GetABPath();
        if (!Directory.Exists(resPath))
            Directory.CreateDirectory(resPath);

        if (Packager.bAssetBundle)
            BuildPipeline.BuildAssetBundles(resPath, BuildAssetBundleOptions.ChunkBasedCompression, Packager.getBuildTarget());
    }

    //清除AB包
    public static void ClearABFolder()
    {
		string resPath = GetABPath();
        if (Directory.Exists(resPath))
        {
            Directory.Delete(resPath, true);
        }
    }

    /// <summary>
    /// 数据目录
    /// </summary>
    static string AppDataPath
    {
        get { return Application.dataPath.ToLower(); }
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
        }
        foreach (string dir in dirs)
        {
            Recursive(dir);
        }
    }

    public static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }


    public static string GetPlatformFolderForAssetBundles(BuildTarget target)
    {
        switch (target)
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
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformFolderForAssetBundles(RuntimePlatform) function.
            default:
                return null;
        }
    }

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    private static string[] PREFAB_FBX_TEX_MAT_PATH = new string[] { "Bundles/Characters/" };

    private static string[] ONLY_PREFAB_PATH = new string[] {  "Bundles/Effects", "Bundles/Powerup" };

    private static string[] UI_PATH = new string[] { "Bundles/UI" };

    private static string[] MAP_PATH = new string[] { "Bundles/Scene", "Bundles/Map" };

       private static string[] FONT_PATH = new string[] { "Bundles/Font" };
#else
    private static string[] PREFAB_FBX_TEX_MAT_PATH = new string[] { "Bundles\\Characters\\" };

    private static string[] ONLY_PREFAB_PATH = new string[] {  "Bundles\\Effects", "Bundles\\Powerup" };
    private static string[] UI_PATH = new string[] { "Bundles\\UI" };

    private static string[] MAP_PATH = new string[] { "Bundles\\Scene", "Bundles\\Map" };

    private static string[] FONT_PATH = new string[] { "Bundles\\Font" };
#endif

    private enum MarkBundleEnum
    {
        PREFAB_FBX_TEX_MAT_PATH,
        ONLY_PREFAB,    
        PREFAB_TEXTURE,
        ONLY_SCENE,
        ONLY_FONT,
    }

    private static MarkBundleEnum GetMarkType(string assetPath)
    {
        for(int  i =0; i < ONLY_PREFAB_PATH.Length; i++)
        {
            if(assetPath.Contains(ONLY_PREFAB_PATH[i]))
            {
                return MarkBundleEnum.ONLY_PREFAB;
            }
        }
        for (int i = 0; i < UI_PATH.Length; i++)
        {
            if (assetPath.Contains(UI_PATH[i]))
            {
                return MarkBundleEnum.PREFAB_TEXTURE;
            }
        }

        for (int i = 0; i < MAP_PATH.Length; i++)
        {
            if (assetPath.Contains(MAP_PATH[i]))
            {
                return MarkBundleEnum.ONLY_SCENE;
            }
        }

        for (int i = 0; i < FONT_PATH.Length; i++)
        {
            if (assetPath.Contains(FONT_PATH[i]))
            {
                return MarkBundleEnum.ONLY_FONT;
            }
        }

        for (int i = 0; i < PREFAB_FBX_TEX_MAT_PATH.Length; i++)
        {
            if (assetPath.Contains(PREFAB_FBX_TEX_MAT_PATH[i]))
            {
                return MarkBundleEnum.PREFAB_FBX_TEX_MAT_PATH;
            }
        }

        return MarkBundleEnum.ONLY_PREFAB;
    }

    public static void BuildAssetMarks()
    {
        string sourcePath = Application.dataPath + "/Bundles";
        List<string> fileSystemEntries = new List<string>();

        fileSystemEntries
            .AddRange(Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)
                          .Select(d => d + "\\"));
        fileSystemEntries
            .AddRange(Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories));

        for (int i = 0; i < fileSystemEntries.Count; i++ )
        {
            string filePath = fileSystemEntries[i];
            string extend = Path.GetExtension(filePath);
            string ext = extend.ToLower();

            string assetPath = filePath.Replace(Application.dataPath, "Assets");
      
            AssetImporter import = AssetImporter.GetAtPath(assetPath);
            if (import != null)
            {
                MarkBundleEnum markEnum = GetMarkType(assetPath);
                switch(markEnum)
                {
                    case MarkBundleEnum.ONLY_PREFAB:
                        if (!ext.Equals(".cs") && !ext.Equals(".prefab"))
                        {
                            import.assetBundleName = "";
                            continue;
                        }
                        break;

                    case MarkBundleEnum.PREFAB_TEXTURE:
                        if (!ext.Equals(".cs") && !ext.Equals(".prefab") && !ext.Equals(".png"))
                        {
                            import.assetBundleName = "";
                            continue;
                        }
                        break;

                    case MarkBundleEnum.ONLY_SCENE:
                        if (!ext.Equals(".cs") && !ext.Equals(".unity"))
                        {
                            import.assetBundleName = "";
                            continue;
                        }
                        break;

                    case MarkBundleEnum.ONLY_FONT:
                        if (!ext.Equals(".otf") && !ext.Equals(".ttf"))
                        {
                            import.assetBundleName = "";
                            continue;
                        }
                        break;
                    case MarkBundleEnum.PREFAB_FBX_TEX_MAT_PATH:
                        if (!ext.Equals(".png") && !ext.Equals(".tga") && !ext.Equals(".prefab") && !ext.Equals(".mat"))
                        {
                            import.assetBundleName = "";
                            continue;
                        }
                        break;
                }

                if (ext.Equals(".prefab") || ext.Equals(".png") || ext.Equals(".mat") || ext.Equals(".unity") || ext.Equals(".fbx") || ext.Equals(".tga") || ext.Equals(".mp3") || ext.Equals(".wav") || ext.Equals(".ttf") || ext.Equals(".otf") || ext.Equals(".anim") || ext.Equals(".controller"))
                {
                    if (!ext.Equals(".png"))
                    {
                        assetPath = assetPath.Replace(extend, "");
                    }
                    else
                    {
                        assetPath = assetPath.Replace(ext, ".img");
                    }
                    import.assetBundleName = assetPath;
                }
                else
                {
                    if(!ext.Equals(".cs"))
                    {
                        import.assetBundleName = "";
                    }
                }
                UpdateProgress(i, fileSystemEntries.Count, assetPath);
            }
        }
        EditorUtility.ClearProgressBar();
    }

    public static void WritePreloadFile()
    {
        string sourcePath = Application.dataPath + "/Bundles";
        List<string> fileSystemEntries = new List<string>();

        fileSystemEntries.AddRange(Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories).Select(d => d + "\\"));
        fileSystemEntries.AddRange(Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories));
        PreloadFileModel fileList = new PreloadFileModel();
        for (int i = 0; i < fileSystemEntries.Count; i++)
        {
            string filePath = fileSystemEntries[i];
            string ext = Path.GetExtension(filePath).ToLower();

            string assetPath = filePath.Replace(Application.dataPath, "Assets");
            AssetImporter import = AssetImporter.GetAtPath(assetPath);
            if (import != null)
            {
                assetPath = assetPath.Replace(ext, "").Replace("\\", "/");
                if (ext.Equals(".prefab")&&!assetPath.Contains("Cell/Skin/"))
                {
                    fileList.Add(assetPath);
                }

                UpdateProgress(i, fileSystemEntries.Count, assetPath);
            }
        }

        string json = JsonUtility.ToJson(fileList, true);

        File.WriteAllText(Application.streamingAssetsPath + "/Config/AllAB.json", json);

        PreloadFileModel outObj = JsonUtility.FromJson<PreloadFileModel>(json);

        EditorUtility.ClearProgressBar();        
    }

	//调用命令行
	public static void ProcessCommand(string command, string argument)
	{
		System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command);
		info.Arguments = argument;
		info.CreateNoWindow = false;
		info.ErrorDialog = true;
		info.UseShellExecute = true;

		if (info.UseShellExecute)
		{
			info.RedirectStandardOutput = false;
			info.RedirectStandardError = false;
			info.RedirectStandardInput = false;
		}
		else
		{
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			info.RedirectStandardInput = true;
			info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
			info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
		}

		System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

		if (!info.UseShellExecute)
		{
			UnityEngine.Debug.LogError(process.StandardOutput);
			UnityEngine.Debug.LogError(process.StandardError);
		}

		process.WaitForExit();
		process.Close();
	}
}

