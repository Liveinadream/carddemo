using Godot;
using System;

public partial class GameEvents
{

    // 商店卡牌点击后的事件展示
    public static event Action<string> OnShowDia;

    public static void RaiseOnShowDia(string diaName)
    {
        OnShowDia?.Invoke(diaName);
    }
}