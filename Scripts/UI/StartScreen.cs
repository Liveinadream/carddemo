using Godot;
using System;

public partial class StartScreen : Control
{
    [Export]
    public Button StartGame;

    [Export]
    public Button LoadGame;

    public override void _Ready()
    {
        base._Ready();
        StartGame.ButtonDown += OnStartGamePressed;
        LoadGame.ButtonDown += OnLoadGamePressed;
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
}
