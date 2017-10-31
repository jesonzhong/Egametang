using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ========================================  时间推送 ========================================  
/// </summary>
public class EventDispatcher
{
    protected static EventDispatcher _instance;

    public Dictionary<string, EventItem> _standingMsgMap = new Dictionary<string, EventItem>(); //已注册的常驻事件集合

    public Dictionary<string, EventItem> _onceMsgMap = new Dictionary<string, EventItem>();     //已注册的单次事件集合

    public static EventDispatcher Instance
    {
        get{
            if (_instance == null)
            {
                _instance = new EventDispatcher();
            }

            return _instance;
         }
    }
    
    
    /// <summary>
    /// 注册常住事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="fn">回调</param>
    public void On(string eventName, EventMethod fn)
    {
        _addListener(eventName, fn, _standingMsgMap);
    }



    /// <summary>
    /// 注销事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="fn"></param>
    public void Off(string eventName, EventMethod fn)
    {
        EventItem item;
        _standingMsgMap.TryGetValue(eventName, out item);
        if (item != null)
        {
            item.Remove(fn);
        }

    }


    /// <summary>
    /// 注册单次事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="fn"></param>
    public void Once(string eventName, EventMethod fn)
    {
        _addListener(eventName, fn, _onceMsgMap);
    }
    

    /// <summary>
    /// 发送事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="args"></param>
    public void Emit(string eventName, params object[] args)
    {
        EventItem item;
        _onceMsgMap.TryGetValue(eventName, out item);
        if (item != null)
        {
            item.Call(args);
            _onceMsgMap.Remove(eventName);
        }

        _standingMsgMap.TryGetValue(eventName, out item);
        if (item != null)
        {
            item.Call(args);
        }
    }


    /// <summary>
    /// 清除所有事件
    /// </summary>
    public void ClearAll()
    {
        _onceMsgMap.Clear();
        _standingMsgMap.Clear();
    }


    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="fn"></param>
    /// <param name="bOnce"></param>
    private void _addListener(string eventName, EventMethod fn, Dictionary<string, EventItem> eventMap)
    {
        EventItem item;
        eventMap.TryGetValue(eventName, out item);
        if (item == null)
        {
            item = new EventItem();
            eventMap.Add(eventName, item);
        }
        item.Add(fn);

    }

}

#region 事件单位

public delegate void EventMethod(params object[] args); //回调格式

public class EventItem
{
    private event EventMethod eventArr;

    public void Add(EventMethod fn)
    {
        if (eventArr != fn)
        {
            eventArr += fn;
        }
    }

    public void Remove(EventMethod fn)
    {
        eventArr -= fn;
    }

    public void Call(params object[] args)
    {
        if (eventArr != null)
        {
            eventArr(args);
        }
    }
};

#endregion