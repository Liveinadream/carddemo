using CardDemo.Map;
using Godot;
using System;

public partial class GameEvents
{

    // 商店卡牌点击后的事件展示
    public static event Action<string> OnShowDia;
    public static event Action OnMapSegmentLoaded;
    public static event Action<MapNodeData> OnMapNodeSelected;
    public static event Action OnEventCompleted;

    public static void RaiseOnShowDia(string diaName)
    {
        OnShowDia?.Invoke(diaName);
    }

    public static void RaiseOnMapSegmentLoaded()
    {
        OnMapSegmentLoaded?.Invoke();
    }

    public static void RaiseOnMapNodeSelected(MapNodeData nodeData)
    {
        OnMapNodeSelected?.Invoke(nodeData);
    }

    public static void RaiseOnEventCompleted()
    {
        OnEventCompleted?.Invoke();
    }
}