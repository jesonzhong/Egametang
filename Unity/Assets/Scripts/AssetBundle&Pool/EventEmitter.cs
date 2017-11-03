///方便在类中调用一些常用接口
using UnityEngine;
using System.Collections;

/// <summary>
/// 一般基类
/// </summary>
public class EventEmitter {

    public void On(string eventName, EventMethod fn) { 
        EventDispatcher.Instance.On(eventName, fn);
    }
    public void Off(string eventName, EventMethod fn)
    {
        EventDispatcher.Instance.Off(eventName, fn);
    }

    public void Once(string eventName, EventMethod fn)
    {
        EventDispatcher.Instance.Once(eventName, fn);
    }
    public void Emit(string eventName)
    {
        EventDispatcher.Instance.Emit(eventName);
    }
}

/// <summary>
/// Mono基类
/// </summary>
public class UEventEmitter : MonoBehaviour
{
    public void On(string eventName, EventMethod fn)
    {
        EventDispatcher.Instance.On(eventName, fn);
    }
    public void Off(string eventName, EventMethod fn)
    {
        EventDispatcher.Instance.Off(eventName, fn);
    }

    public void Once(string eventName, EventMethod fn)
    {
        EventDispatcher.Instance.Once(eventName, fn);
    }
    public void Emit(string eventName, params object[] args)
    {
        EventDispatcher.Instance.Emit(eventName, args);
    }

}