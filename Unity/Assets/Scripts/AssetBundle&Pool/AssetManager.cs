
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// 预加载的assetbundle的集合
/// </summary>
public class PreloadFileModel
{
    public List<string> fileList = new List<string>();

    public void Add(string file)
    {
        if (!fileList.Contains(file))
        {
            fileList.Add(file);
        }
    }
}

/// <summary>
/// 资源管理类
/// </summary>
public class AssetManager : UEventEmitter
{
    private static AssetManager _instance;

    private AssetCachePool _longTermAssetPool;
    private AssetCachePool _shortAssetPool;

    private AssetCachePool _outBattleEntityAssetPool;
    private AssetCachePool _inBattleEntitAssetPool;

    private const string _cellEnityPath = "Assets/Bundles/Entity/";

    private const string _prefabPath = "Assets/Bundles/"; 

    public const string _prefabPathLower = "assets/bundles/"; 

    private const string _uipath = "Assets/Bundles/UI/"; 

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    public static AssetManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Init()
    {
        _longTermAssetPool = AddAssetPool("_longTermAssetPool");
        _shortAssetPool = AddAssetPool("_shortAssetPool");
        _inBattleEntitAssetPool = AddAssetPool("_inBattleEntitAssetPool");
        _outBattleEntityAssetPool = AddAssetPool("_outBattleEntityAssetPool");
    }

    private AssetCachePool AddAssetPool(string poolName)
    {
        GameObject go = new GameObject(poolName);
        go.transform.SetParent(this.transform);
        return go.AddComponent<AssetCachePool>();
    }

    #region 常驻资源
    /// <summary>
    /// 预加载游戏
    /// </summary>
    /// <param name="cb">回调:返回读取进度</param>
    public void PreloadAsset(CallBackWithPercent cb)
    {
        PreloadFileModel preloadModel = FileUtil.ReadJsonFromFile<PreloadFileModel>("Config/PreloadAB.json");
        _longTermAssetPool.LoadAssetList(preloadModel, cb);
    }

    public GameObject GetTempPrefab(string assetName)
    {
        return _shortAssetPool.GetPrefab(_uipath + assetName);
    }

    public GameObject GetUIPrefab(string assetName)
    {
        return GetTempPrefab(assetName);
    }

    #endregion

    #region 临时资源

    public void LoadMaterialFullPath(string assetName, Action<Material> cb)
    {
        _inBattleEntitAssetPool.LoadMaterial(assetName, cb);
    }

    public void GetEntityPrefabAsync(string assetName, Action<GameObject> cb)
    {
        _shortAssetPool.LoadAsset(_cellEnityPath + assetName, cb);
    }


    public GameObject GetPrefabFullPath(string assetName, bool needTolower = true)
    {
        return _inBattleEntitAssetPool.GetPrefab(assetName, needTolower);
    }

    public GameObject GetPrefab(string assetName, bool needTolower = true)
    {
        if(needTolower)
        {
            return GetPrefabFullPath(_prefabPath + assetName, needTolower);
        }
        else
        {
            return GetPrefabFullPath(_prefabPathLower + assetName, needTolower);
        }
    }

    public void CleanInBattlePoolAssets()
    {
        _inBattleEntitAssetPool.CleanUp();
    }

    public void CleanOutBattlePoolAssets()
    {
        //_outBattleEntityAssetPool.CleanUp();
    }

    public void CleanShortAssets()
    {
        //_shortAssetPool.CleanUp();
    }
    #endregion

    public void CleanEnityPool()
    {
       
    }


    private Dictionary<string, Dictionary<string, Sprite>> _spriteDictDict = new Dictionary<string, Dictionary<string, Sprite>>();

    /// <summary>
    /// 异步加载精灵
    /// </summary>
    /// <param name="path">贴图路径</param>
    /// <param name="spName">精灵名称</param>
    /// <param name="fn">返回精灵回调</param>
    public void LoadSpriteAsync(string path, string spName, Action<Sprite> fn)
    {
        Sprite sprite = null;
        Dictionary<string, Sprite> dict = null;
        if (_spriteDictDict.TryGetValue(path, out dict))
        {
            dict.TryGetValue(spName, out sprite);
            fn(sprite);
        }
        else
        {
            string bundleName = path.ToLower() + ".img";
            string assetName = path + ".png";
            AssetBundleLoader.Instance.LoadAllAsset(bundleName, assetName,
            (arr) =>
            {
                if (!_spriteDictDict.TryGetValue(path, out dict))
                {
                    dict = new Dictionary<string, Sprite>();
                    _spriteDictDict.Add(path, dict);
                    //返回一个精灵列表
                    if (arr != null)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            Sprite obj = arr[i] as Sprite;
                            if (obj != null)
                            {
                                dict.Add(obj.name, obj);
                            }
                        }
                    }
                }

                dict.TryGetValue(spName, out sprite);

                fn(sprite);
            });
        }
    }
}
