using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


public class EffectManager
{
	private static EffectManager _instance;
	public static EffectManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new EffectManager();
			}
			return _instance;
		}
	}

    private BetterDictionary<string, GameObject> _fxDict;

    public GameObject AddFxAutoRemove(string fxPath, Transform parent = null, float duration = 5f, Vector3 localPos = default(Vector3), Vector3 localRotation = default(Vector3))
    {
        GameObject gp = AddFx(fxPath, parent, localPos, localRotation);
        DelayManager.instance.delay<string>(duration, fxPath, RemoveFx);
        return gp;
    }

    public GameObject AddFx(string fxPath, Transform parent = null, Vector3 localPos = default(Vector3), Vector3 localRotation = default(Vector3))
    {
        if (_fxDict == null)
        {
            _fxDict = new BetterDictionary<string, GameObject>();
        }

        GameObject go = null;
        _fxDict.TryGetValue(fxPath, out go);
        if (go != null)
        {
            return go;
        }

        GameObject prefab = AssetManager.Instance.GetPrefab(fxPath);
        if (prefab == null)
        {
            return null;
        }

        go = PoolManager.Spawn(parent, prefab, Vector3.zero, Quaternion.identity);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.Euler(localRotation);
        go.transform.localScale = Vector3.one;
        _fxDict.Add(fxPath, go);
        return go;
    }

    public void RemoveFx(string fxPath)
    {
        if (_fxDict == null)
        {
            return;
        }

        GameObject go = null;
        _fxDict.TryGetValue(fxPath, out go);
        if (go != null)
        {
            PoolManager.Despawn(go);
            _fxDict.Remove(fxPath);
        }
        else
        {
            //SampleDebuger.LogWarning("RemoveFx:" + fxPath);
        }
    }

    private void RemoveAllFx()
    {
        if (_fxDict == null)
        {
            return;
        }

        var pairsKey = _fxDict.KeyValuePairs;
        for (int i = 0; i < pairsKey.Count; i++)
        {
            RemoveFx(pairsKey[i].Key);
        }
    }
}
