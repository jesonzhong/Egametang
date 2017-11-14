using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;

public class GameVersion
{

    string _curVersion = "";

    public int mainVersion = 0;

    public int subVersion = 0;

    public int miniVersion = 0;

    public GameVersion()
    {
    }

    public GameVersion(string ver)
    {

        curVersion = ver;
    }

    public string curVersion
    {

        get
        {

            return string.Format("{0}.{1}.{2}", mainVersion, subVersion, miniVersion);
        }
        set
        {
            string tmp = value;
            _curVersion = tmp;
            string[] t = tmp.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (t.Length > 2)
            {
                mainVersion = int.Parse(t[0]);
                subVersion = int.Parse(t[1]);
                miniVersion = int.Parse(t[2]);
            }
        }
    }

    override public string ToString()
    {
        return curVersion;
    }

    public int ToNumber()
    {
        return mainVersion * 10000 + subVersion * 100 + miniVersion ;
    }

    public bool IsLower(GameVersion other)
    {
		return ToNumber() < other.ToNumber();
    }

	public bool IsEqual(GameVersion other){
		return mainVersion == other.mainVersion && 
			subVersion == other.subVersion && 
			miniVersion == other.miniVersion;
	
	}
}


public class VersionManager
{

    static VersionManager _instance = new VersionManager();

    public GameVersion version = new GameVersion("0.0.0");

    public static VersionManager Instance
    {

        get
        {
            
#if UNITY_EDITOR

            // 读取 version.txt
            byte[] content = File.ReadAllBytes(Application.streamingAssetsPath + "/version.txt");

            string ver = System.Text.Encoding.UTF8.GetString(content);
            //StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/version.txt",Encoding.Default);
            _instance.curVersion = ver;
            //sr.Close();

#endif

            return _instance;
        }
    }

    public string curVersion
    {

        get
        {

            return version.curVersion;
        }
        set
        {

            version.curVersion = value;

#if UNITY_EDITOR
            
            //saveVersion(Application.streamingAssetsPath + "/version.txt");
#else
            saveVersion(Application.persistentDataPath + "/version.txt");
#endif

        }
    }

    public string srvVersion  { get; set; }

    public int getVersionNum()
    {
        return version.ToNumber();
    }

    public string getVersionUrl()
    {
        return curVersion.Replace(".", "_");
    }

    public void SaveImage(Texture2D image)
    {
        string path = Application.persistentDataPath + "/paopaoerweima.png";
        Byte[] data = image.EncodeToPNG();
        File.WriteAllBytes(path, data);
    }

    public void saveVersion(string path)
    {
        File.WriteAllBytes(path, System.Text.Encoding.UTF8.GetBytes(version.ToString()));
		//SampleDebuger.Log(" Update version info");
    }
}
