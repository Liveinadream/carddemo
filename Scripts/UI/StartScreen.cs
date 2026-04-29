using Godot;
using System;

public partial class StartScreen : Control
{
    [Export]
    public Button StartGame;

    [Export]
    public Button LoadGame;
    [Export]
    public Button GotoMap;

    public override void _Ready()
    {
        base._Ready();
        StartGame.ButtonDown += OnStartGamePressed;
        LoadGame.ButtonDown += OnLoadGamePressed;
        GotoMap.ButtonDown += OnGotoMapPressed;
    }

    // 新游戏按钮点击逻辑
    private void OnStartGamePressed()
    {
        GlobalManager.GotoNameScreen();
    }

    // 继续游戏按钮点击逻辑
    private void OnLoadGamePressed()
    {
        GlobalManager.GetPauseMenu().ShowPauseMenu();
        GlobalManager.GetPauseMenu().OnSaveListButtonDown();
    }

    // 进入地图按钮点击逻辑
    private void OnGotoMapPressed()
    {
        GlobalManager.GotoMapScreen();
    }
}
