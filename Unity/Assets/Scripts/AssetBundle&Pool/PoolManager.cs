using System.Collections.Generic;
using UnityEngine;


public class PoolManager// : MonoBehaviour
{
    private static BetterDictionary<GameObject, Pool> Pools_Prefab = new BetterDictionary<GameObject, Pool>();

    private static Dictionary<GameObject, Pool> Pools_Instance = new Dictionary<GameObject, Pool>(100);

    public GameObject gameObject
    {
        get
        {
            return _gameObject;
        }
    }

    public Transform transform
    {
        get
        {
            return _transform;
        }
    }

    private GameObject _gameObject;
    private Transform _transform;

    private static PoolManager s_instance;
    private static PoolManager instance
    {
        get
        {
            if(s_instance == null)
            {
                s_instance = new PoolManager();
                s_instance._gameObject = new GameObject("PoolManager");
                s_instance._transform = s_instance._gameObject.transform;
                GameObject.DontDestroyOnLoad(s_instance._gameObject);
            }
            return s_instance;
        }
    }

    /// <summary>
    /// 实例对应某对象池映射
    /// </summary>
    /// <param name="instanceObj"></param>
    /// <param name="pool"></param>
    public static void AddInstancePoolMap(GameObject instanceObj, Pool pool)
    {
        if(instanceObj != null && pool != null)
        {
            if(Pools_Instance.ContainsKey(instanceObj))
            {
                Pools_Instance[instanceObj] = pool;
            }
            else
            {
                Pools_Instance.Add(instanceObj, pool);
            }
        }
    }

    /// <summary>
    /// 实例对应某对象池映射
    /// </summary>
    /// <param name="instanceObj"></param>
    /// <param name="pool"></param>
    public static void RemoveInstancePoolMap(GameObject instanceObj)
    {
        if (instanceObj != null)
        {
            if (Pools_Instance.ContainsKey(instanceObj))
            {
                Pools_Instance.Remove(instanceObj);
            }
        }
    }

    public static void Add(Pool pool)
    {
        if (pool.prefab == null)
        {
            Debug.LogError("Prefab of pool: " + pool.gameObject.name + " is empty! Can't add pool to Pools Dictionary.");
            return;
        }

        if (Pools_Prefab.ContainsKey(pool.prefab))
        {
            //Debug.LogError("Pool with prefab " + pool.prefab.name + " has already been added to Pools Dictionary.");
            return;
        }

        Pools_Prefab.Add(pool.prefab, pool);
    }


    public static void CreatePool(GameObject prefab, int preLoad, bool limit, int maxCount)
    {
        if (Pools_Prefab.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager already contains Pool for prefab: " + prefab.name);
            return;
        }
#if UNITY_EDITOR
        GameObject newPoolGO = new GameObject("Pool " + prefab.name);
#else
        GameObject newPoolGO = new GameObject();
#endif
    
        if (instance != null)
        {
            newPoolGO.transform.SetParent(instance.transform);
        }

        //DontDestroyOnLoad(newPoolGO);

        Pool newPool = newPoolGO.AddComponent<Pool>();
        newPool.prefab = prefab;
        newPool.preLoad = preLoad;
        newPool.limit = limit;
        newPool.maxCount = maxCount;
        newPool.OnInit();
    }

    /// <summary>
    /// 带自动回收 减少消耗
    /// </summary>
    /// <returns></returns>
    public static GameObject SpawnAutoDespawn(GameObject prefab, Vector3 position, Quaternion rotation , float time)
    {
        Pool pool = null;
        GameObject instance = null;
        if (prefab == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Spawn error!");
#endif
        }
        else if (!Pools_Prefab.ContainsKey(prefab))
        {
            CreatePool(prefab, 0, false, 0);
            pool = Pools_Prefab[prefab];
        }
        else
        {
            pool = Pools_Prefab[prefab];
        }

        if (pool == null)
        {
            instance = new GameObject("Error");
        }
        instance = pool.Spawn(position, rotation);

        if (time > 0)
        {
            pool.Despawn(instance, time);
        }
        else
        {
            pool.Despawn(instance);
        }
        return instance;
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Pool pool = null;
        if (prefab == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Spawn error!");
#endif
        }
        else if (!Pools_Prefab.ContainsKey(prefab))
        {
            CreatePool(prefab, 0, false, 0);
            pool = Pools_Prefab[prefab];
        }
        else
        {
            pool = Pools_Prefab[prefab];
        }

        if (pool == null)
        {
            return new GameObject("Error");
        }
        return pool.Spawn(position, rotation);
    }

    public static GameObject Spawn(Transform parent, GameObject prefab, Vector3 localPosition, Quaternion localRotation)
    {
        GameObject go = Spawn(prefab, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(parent.transform);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = localRotation;
        return go;
    }

    public static void Despawn(GameObject instance, float time = 0f)
    {
        if (instance == null)
        {
            return;
        }

        Pool pool = GetPool(instance);
        if (pool == null)
        {
            instance.SetActive(false);
            return;
        }

        if (time > 0)
        {
            pool.Despawn(instance, time);
        }
        else
        {
            pool.Despawn(instance);
        }
    }

    public static Pool GetPool(GameObject instance)
    {
        if(instance != null)
        {
            Pool pool = null;
            Pools_Instance.TryGetValue(instance, out pool);
            return pool;
        }
        return null;

       // var enumerator = Pools_Prefab.GetEnumerator();
       //while(enumerator.MoveNext())
       // {
       //     var list = enumerator.Current.Value.active;
       //     for(int i =0; i < list.Count; i++)
       //     {
       //         if(list[i] == instance)
       //         {
       //             return enumerator.Current.Value;
       //         }
       //     }
       // }
       // return null;
    }


    public static void DeactivatePool(GameObject prefab)
    {
        if (!Pools_Prefab.ContainsKey(prefab))
        {
            Debug.LogError("PoolManager couldn't find Pool for prefab to deactivate: " + prefab.name);
            return;
        }

        int count = Pools_Prefab[prefab].active.Count;
        for (int i = count - 1; i > 0; i--)
        {
            Pools_Prefab[prefab].Despawn(Pools_Prefab[prefab].active[i]);
        }
    }


    public static void DestroyAllInactive(bool limitToPreLoad)
    {
        Pools_Prefab.BetterForeach((pair) =>
        {
            pair.Value.DestroyUnused(limitToPreLoad);
        });
    }

    public static void DestroyPool(GameObject prefab)
    {
        if (!Pools_Prefab.ContainsKey(prefab))
        {
            Debug.LogError("PoolManager couldn't find Pool for prefab to destroy: " + prefab.name);
            return;
        }

        if(prefab != null && Pools_Prefab[prefab] != null)
        {
            GameObject.Destroy(Pools_Prefab[prefab].gameObject);
        }
        Pools_Prefab.Remove(prefab);
    }


    public static void DestroyAllPools()
    {
        //var it = Pools.GetEnumerator();
        List<KeyValuePair<GameObject, Pool>> keyValuePairs = null;
        Pools_Prefab.GetKeyValuePairs(out keyValuePairs);

        if(keyValuePairs != null)
        {
            for(int  i = 0; i < keyValuePairs.Count; i++ )
            {
                if(keyValuePairs[i].Key != null)
                {
                    GameObject go = keyValuePairs[i].Key;
                    DestroyPool(go);
                }
              
            }
        }
    }

    void OnDestroy()
    {
        Pools_Prefab.Clear();
        Pools_Instance.Clear();
    }
}
