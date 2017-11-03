using System;
using System.Collections.Generic;

public class BetterDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    public delegate bool CheckList(KeyValuePair<TKey, TValue> map);
    private List<KeyValuePair<TKey, TValue>> _keyValuePairs = new List<KeyValuePair<TKey, TValue>>();

    /// <summary>
    /// 循环代理
    /// </summary>
    /// <param name="index">循环次数</param>
    /// <param name="item">字典内元素</param>
    /// <returns> 循环控制 true 继续  false Break </returns>
    public delegate bool ForeachHandle(int index, KeyValuePair<TKey, TValue> item);

    public List<KeyValuePair<TKey, TValue>> KeyValuePairs
    {
        get
        {
            _keyValuePairs.Clear();
            var e = this.GetEnumerator();

            while (e.MoveNext())
            {
                TKey key = ((e.Current)).Key;
                TValue value = ((e.Current)).Value;
                KeyValuePair<TKey, TValue> kvp = new KeyValuePair<TKey, TValue>(key, value);
                _keyValuePairs.Add(kvp);
            }
            e.Dispose();
            return _keyValuePairs;
        }
    }

    public void GetKeyValuePairs(out List<KeyValuePair<TKey, TValue>> keyValuePairs)
    {
        keyValuePairs = new List<KeyValuePair<TKey, TValue>>();
        
        var e = this.GetEnumerator();

        while (e.MoveNext())
        {
            TKey key = ((e.Current)).Key;
            TValue value = ((e.Current)).Value;
            KeyValuePair<TKey, TValue> kvp = new KeyValuePair<TKey, TValue>(key, value);
            keyValuePairs.Add(kvp);
        }
        e.Dispose();
    }

    List<KeyValuePair<TKey, TValue>> keyValuePairs = new List<KeyValuePair<TKey, TValue>>();
    public void BetterForeach(Action<KeyValuePair<TKey, TValue>> callback)
    {
        keyValuePairs.Clear();

        var e = this.GetEnumerator();

        while (e.MoveNext())
        {
            TKey key = ((e.Current)).Key;
            TValue value = ((e.Current)).Value;
            KeyValuePair<TKey, TValue> kvp = new KeyValuePair<TKey, TValue>(key, value);
            keyValuePairs.Add(kvp);
        }
        e.Dispose();

        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            callback(keyValuePairs[i]);
        }
        keyValuePairs.Clear();
    }

    public bool CheckBetterForeach(CheckList callback)
    {
        keyValuePairs.Clear();

        var e = this.GetEnumerator();

        while (e.MoveNext())
        {
            TKey key = ((e.Current)).Key;
            TValue value = ((e.Current)).Value;
            KeyValuePair<TKey, TValue> kvp = new KeyValuePair<TKey, TValue>(key, value);
            keyValuePairs.Add(kvp);
        }
        e.Dispose();

        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            if (false == callback(keyValuePairs[i]))
            {
                keyValuePairs.Clear();
                return false;
            }
        }
        keyValuePairs.Clear();
        return true;
    }

    public void BetterForeach( Action<KeyValuePair<TKey, TValue>> callback, int indexEnd, int indexSatrt = 0)
    {
        keyValuePairs.Clear();

        var e = this.GetEnumerator();

        while (e.MoveNext())
        {
            TKey key = ((e.Current)).Key;
            TValue value = ((e.Current)).Value;
            KeyValuePair<TKey, TValue> kvp = new KeyValuePair<TKey, TValue>(key, value);
            keyValuePairs.Add(kvp);
        }
        e.Dispose();
        indexEnd = indexEnd > keyValuePairs.Count ? keyValuePairs.Count : indexEnd;
        for (int i = indexSatrt; i < indexEnd; i++)
        {
            callback(keyValuePairs[i]);
        }
        keyValuePairs.Clear();
    }
    public void BetterForeach(ForeachHandle callback)
    {
        keyValuePairs.Clear();

        var e = this.GetEnumerator();

        while (e.MoveNext())
        {
            TKey key = ((e.Current)).Key;
            TValue value = ((e.Current)).Value;
            KeyValuePair<TKey, TValue> kvp = new KeyValuePair<TKey, TValue>(key, value);
            keyValuePairs.Add(kvp);
        }
        e.Dispose();

        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            if (callback(i, keyValuePairs[i]) == false) break;
        }
        keyValuePairs.Clear();
    }

    private List<TKey> RmoveCache=null;
    public void CacheRemove(TKey key)
    {
        if (RmoveCache == null) RmoveCache = new List<TKey>();
        RmoveCache.Add(key);
    }
    public void RemoveFromCache()
    {
        if (RmoveCache == null) return;
        for (int i = 0; i < RmoveCache.Count; i++)
        {
            this.Remove(RmoveCache[i]);
        }
        RmoveCache.Clear();
    }
    new public void Clear()
    {
        base.Clear();
        if (RmoveCache != null)
            RmoveCache.Clear();
        _keyValuePairs.Clear();
    }
}
