using UnityEngine;
using System.Collections;
using System;

public class UIUtil
{
    public static void SetParent(Transform trans, Transform parent)
    {
        if (parent != null)
        {
            trans.SetParent(parent);
        }
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
    }


    private static void print(string v)
    {
        throw new NotImplementedException();
    }

    public static string ConverSecToString(int seconds)
    {
        DateTime format = new DateTime();
		string str = "";
		if (seconds < 3600) {
			str = format.AddSeconds (Mathf.Clamp (seconds, 0, seconds)).ToString ("mm:ss");
		} 
		else 
		{
			str = format.AddSeconds (Mathf.Clamp (seconds, 0, seconds)).ToString ("HH:mm:ss");	
		}
        return str;
    }

    /// <summary>
    /// H代表24小时制 h代表12小时
    /// <returns></returns>
    public static string ConverSecToString_HMS(int seconds, string _format)
    {
        DateTime format = new DateTime(1970, 1, 1);
        var localTime = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToLocalTime();
        string str = localTime.ToString(_format);
        return str;
    }

    public static string ConverLeftTimeSecToString(int seconds,string _format)
    {
        //DateTime format = new DateTime(1970, 1, 1);

        //string str = format.AddSeconds(Mathf.Clamp(seconds, 0, seconds)).ToString(_format);

        TimeSpan s = new TimeSpan(0, 0, seconds);

        string format = string.Format("{0}天{1}小时{2}分钟", s.Days, s.Hours, s.Minutes);
        return format;
       // return str;
    }


	public static void SetLayerIgnoreChild(Transform t,int _layer)
	{
		t.gameObject.layer = _layer;
	}

    public static void SetLayer(Transform t, int _layer)
    {
        t.gameObject.layer = _layer;
        int count = t.childCount;
        if (count == 0)
        {
            return;
        }
        else
        {
            for (int i = 0; i < count; ++i)
            {
                Transform child = t.GetChild(i);

                SetLayer(child, _layer);
            };
        }
    }


    public static void DestroyChildren(GameObject obj)
    {
        if(obj == null)
        {
            return;
        }
        int count = obj.transform.childCount;
        for (int i = 0; i < count; ++i)
        {
            GameObject.Destroy(obj.transform.GetChild(i).gameObject, 0.1f);
        }
    }

    public static void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }

    public static void HideGoUseScale(GameObject go)
    {
        if (go != null)
        {
            go.transform.localScale = Vector3.zero;
        }
    }
    public static void ShowGoUseScale(GameObject go)
    {
        if(go != null)
        {
            go.transform.localScale = Vector3.one;
        }
    }

}
