//#define TEST_BUNDLE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate void CallBackWithPercent(int done, int total);

public class AssetCachePool : UEventEmitter
{

    private Dictionary<string, GameObject> _cacheAssets = new Dictionary<string, GameObject>();
    private Dictionary<string, Material> _cacheMaterials = new Dictionary<string, Material>();

#if TEST_BUNDLE
    [SerializeField]
    public List<GameObject> _inspectorList = new List<GameObject>();
#endif


#if TEST_BUNDLE
    public void Update()
    {
        _inspectorList.Clear();
       foreach(var pair in _cacheAssets)
        {
            _inspectorList.Add(pair.Value);
        }
    }
#endif

    public void LoadMaterial(string path, Action<Material> fn)
    {
        string bundleName = path;// path.ToLower();
        string assetName = path + ".mat";
        Material material = null;
        _cacheMaterials.TryGetValue(bundleName, out material);
        if (material != null)
        {
            fn(material);
        }
        else
        {
            AssetBundleLoader.Instance.LoadAsset(bundleName, assetName, (assetObj) =>
            {
                material = assetObj as Material;
                if (!_cacheMaterials.ContainsKey(bundleName))
                {
                    _cacheMaterials.Add(bundleName, material);
                }
                fn(material);
            });
        }

    }

    public void LoadAsset(string path, Action<GameObject> fn)
	{
		string bundleName = path.ToLower();
		string assetName = path + ".prefab";
		GameObject prefab = null;
		_cacheAssets.TryGetValue(bundleName, out prefab);
		if (prefab != null)
		{
			fn(prefab);
		}
		else
		{
			AssetBundleLoader.Instance.LoadAsset(bundleName, assetName, (assetObj) =>
				{
					prefab = assetObj as GameObject;
					if (!_cacheAssets.ContainsKey(bundleName))
					{
						_cacheAssets.Add(bundleName, prefab);
					}
					fn(prefab);
				});
		}

	}

	public void LoadAssetList(PreloadFileModel model, CallBackWithPercent cb)
	{
		StartCoroutine(OnLoadAssetList(model, cb));
	}

	private IEnumerator OnLoadAssetList(PreloadFileModel model, CallBackWithPercent cb)
	{
		List<string> list = model.fileList;

		for (int i = 0; i < list.Count; i++)
		{
			string path = list[i];
			string bundleName = path.ToLower();
			string assetName = path + ".prefab";

			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(bundleName, assetName, typeof(UnityEngine.Object));
			if (request != null)
			{
				yield return StartCoroutine(request);
				GameObject obj = request.GetAsset<UnityEngine.GameObject>();
				_cacheAssets.Add(bundleName, obj);
			}
			else
			{
				Debug.LogError("bundle ++" + bundleName + "++ can't loading");
			}
			cb(i + 1, list.Count);
		}
	}

	//获得Prefab
	public GameObject GetPrefab(string path, bool needTolower = true)
	{
		GameObject obj = null;
        if(needTolower)
        {
            path = path.ToLower();
        }
        _cacheAssets.TryGetValue(path, out obj);

		if (obj == null)
		{
			string bundleName = path;
            string assetName = path + ".prefab"; 
             obj = AssetBundleManager.LoadAssetSync(bundleName, assetName, typeof(UnityEngine.Object));

			if (obj != null)
				_cacheAssets.Add(bundleName, obj);
		}
		return obj;
	}

	//加载Prefab通过回调返回
	public void GetPrefabAsync(string path, Action<GameObject> fn)
	{
		GameObject obj = GetPrefab(path.ToLower());
		if (obj == null)
		{
			LoadAsset(path, (assetObj) =>
				{
					fn(assetObj);
				});
		}
		else
		{
			fn(obj);
		}
	}


	public void CleanUp()
	{
		foreach (string key in _cacheAssets.Keys)
		{
			AssetBundleManager.UnloadAssetBundle(key, true);
		}
		_cacheAssets.Clear();

        foreach (string key in _cacheMaterials.Keys)
        {
            AssetBundleManager.UnloadAssetBundle(key, true);
        }
        _cacheMaterials.Clear();
    }

}
