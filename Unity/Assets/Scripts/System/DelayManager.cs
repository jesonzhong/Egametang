using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class DelayManager : MonoBehaviour
{

    public static DelayManager s_instance;
    public static DelayManager instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject go = new GameObject();
                go.name = "DelayManager";
                DontDestroyOnLoad(go);
                s_instance = go.AddComponent<DelayManager>();
            }
            return s_instance;
        }
    }

    #region 通用延迟回调函数
    /// <summary>
    /// 无参数延迟回调
    /// </summary>
    /// <param name="delayTime">延迟时间</param>
    /// <param name="handler">回调函数</param>
    public void delay(float delayTime, Action handler)
    {

        StartCoroutine(beginDelay(delayTime, handler));

    }


    /// <summary>
    /// 1个参数延迟回调
    /// </summary>
    /// <param name="delayTime">延迟时间</param>
    /// <param name="arg1">回调参数1</param>
    /// <param name="handler">回调函数</param>
    public void delay<T>(float delayTime, T arg1, Action<T> handler)
    {

        StartCoroutine(beginDelay<T>(delayTime, arg1, handler));

    }

    /// <summary>
    /// 2个参数延迟回调
    /// </summary>
    /// <param name="delayTime">延迟时间</param>
    /// <param name="arg1">回调参数1</param>
    /// <param name="arg2">回调参数2</param>
    /// <param name="handler">回调函数</param>
    public void delay<T, U>(float delayTime, T arg1, U arg2, Action<T, U> handler)
    {

        StartCoroutine(beginDelay<T, U>(delayTime, arg1, arg2, handler));

    }

    /// <summary>
    /// 3个参数延迟回调
    /// </summary>
    /// <param name="delayTime">延迟时间</param>
    /// <param name="arg1">回调参数1</param>
    /// <param name="arg2">回调参数2</param>
    /// <param name="arg3">回调参数3</param>
    /// <param name="handler">回调函数</param>
    public void delay<T, U, V>(float delayTime, T arg1, U arg2, V arg3, Action<T, U, V> handler)
    {

        StartCoroutine(beginDelay<T, U, V>(delayTime, arg1, arg2, arg3, handler));

    }

    /// <summary>
    /// 4个参数延迟回调
    /// </summary>
    /// <param name="delayTime">延迟时间</param>
    /// <param name="arg1">回调参数1</param>
    /// <param name="arg2">回调参数2</param>
    /// <param name="arg3">回调参数3</param>
    /// <param name="arg4">回调参数4</param>
    /// <param name="handler">回调函数</param>
    public void delay<T, U, V, W>(float delayTime, T arg1, U arg2, V arg3, W arg4, Action<T, U, V, W> handler)
    {

        StartCoroutine(beginDelay<T, U, V, W>(delayTime, arg1, arg2, arg3, arg4, handler));

    }

    IEnumerator beginDelay(float delayTime, Action handler)
    {

        yield return new WaitForSeconds(delayTime);

        handler();
    }

    IEnumerator beginDelay<T>(float delayTime, T arg1, Action<T> handler)
    {

        yield return new WaitForSeconds(delayTime);

        handler(arg1);
    }

    IEnumerator beginDelay<T, U>(float delayTime, T arg1, U arg2, Action<T, U> handler)
    {

        yield return new WaitForSeconds(delayTime);

        handler(arg1, arg2);
    }
    IEnumerator beginDelay<T, U, V>(float delayTime, T arg1, U arg2, V arg3, Action<T, U, V> handler)
    {

        yield return new WaitForSeconds(delayTime);

        handler(arg1, arg2, arg3);
    }

    IEnumerator beginDelay<T, U, V, W>(float delayTime, T arg1, U arg2, V arg3, W arg4, Action<T, U, V, W> handler)
    {

        yield return new WaitForSeconds(delayTime);

        handler(arg1, arg2, arg3, arg4);
    }


    #endregion
}
