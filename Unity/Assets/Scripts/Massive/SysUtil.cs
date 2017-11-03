using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

/// <summary>
/// =============================== 系统工具类 ===============================
/// </summary>
public class SysUtil {

    /// <summary>
    /// 将枚举转换为Int
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static int GetEnumWithInt(object obj)
    {
        if (obj.GetType().IsEnum)
        {
            return (int)obj;
        }
        return 0;
    }


    /// <summary>
    /// 获得当前平台的字符名称
    /// </summary>
    /// <returns></returns>
    public static string GetPlatformName()
    {
        RuntimePlatform platform = Application.platform;
        switch (platform)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                return "OSX";
            default:
                return null;
        }
    }


    /// <summary>
    /// 根据名字获得子节点
    /// </summary>
    /// <param name="root">根节点</param>
    /// <param name="name">名称</param>
    /// <returns>子节点</returns>
    public static GameObject FindChild(GameObject root, string name)
    {

        foreach (Transform tx in root.GetComponentsInChildren<Transform>(true))
        {
            if (tx.name == name)
            {
                return tx.gameObject;
            }
        }
        return null;
    }

    public static string UrlEncode(string str)
    {
        StringBuilder sb = new StringBuilder();

        byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str);

        for (int i = 0; i < byStr.Length; i++ )
        {
            sb.Append(@"%"+Convert.ToString(byStr[i],16));
        }

        return (sb.ToString());
    }

    /// <summary>
    /// 获取屏幕分辨率的宽度
    /// </summary>
    /// <returns></returns>
    public static int GetScreenWidth()
    {
        return Screen.width;
    }


    /// <summary>
    /// 获取屏幕分辨率的高度
    /// </summary>
    /// <returns></returns>
    public static int GetScreenHeight()
    {
        return Screen.height;
    }
}
