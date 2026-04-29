using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 全局事件总线，用于跨模块的解耦通信。
/// </summary>
public partial class EventBus : Singleton<EventBus>
{
    private Dictionary<string, Delegate> _events = new Dictionary<string, Delegate>();

    /// <summary>
    /// 注册不带参数的事件监听。
    /// </summary>
    public void Subscribe(string eventName, Action listener)
    {
        if (!_events.ContainsKey(eventName))
        {
            _events[eventName] = null;
        }
        _events[eventName] = (Action)_events[eventName] + listener;
    }

    /// <summary>
    /// 注册带一个参数的事件监听。
    /// </summary>
    public void Subscribe<T>(string eventName, Action<T> listener)
    {
        if (!_events.ContainsKey(eventName))
        {
            _events[eventName] = null;
        }
        _events[eventName] = (Action<T>)_events[eventName] + listener;
    }

    /// <summary>
    /// 取消不带参数的事件监听。
    /// </summary>
    public void Unsubscribe(string eventName, Action listener)
    {
        if (_events.ContainsKey(eventName))
        {
            _events[eventName] = (Action)_events[eventName] - listener;
        }
    }

    /// <summary>
    /// 取消带一个参数的事件监听。
    /// </summary>
    public void Unsubscribe<T>(string eventName, Action<T> listener)
    {
        if (_events.ContainsKey(eventName))
        {
            _events[eventName] = (Action<T>)_events[eventName] - listener;
        }
    }

    /// <summary>
    /// 触发不带参数的事件。
    /// </summary>
    public void Publish(string eventName)
    {
        if (_events.TryGetValue(eventName, out var del) && del is Action action)
        {
            action.Invoke();
        }
    }

    /// <summary>
    /// 触发带一个参数的事件。
    /// </summary>
    public void Publish<T>(string eventName, T data)
    {
        if (_events.TryGetValue(eventName, out var del) && del is Action<T> action)
        {
            action.Invoke(data);
        }
    }

    /// <summary>
    /// 清除所有事件。
    /// </summary>
    public void ClearAll()
    {
        _events.Clear();
    }
}