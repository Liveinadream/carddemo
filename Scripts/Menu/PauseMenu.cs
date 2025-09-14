using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
    [Export]
    public Panel SavePanel;
    [Export]
    public VBoxContainer SaveContainer;
    [Export]
    public Button BackToGame;
    [Export]
    public Button QuickSave;
    [Export]
    public Button SaveList;
    [Export]
    public Button ExitGame;

    [Export]
    public Button SaveNewButton;
    [Export]
    public Button BackButton;

    public override void _Ready()
    {
        BackToGame.ButtonDown += OnBackToGamePressed;
        QuickSave.ButtonDown += OnQuickSavePressed;
        SaveList.ButtonDown += OnSaveListButtonDown;
        ExitGame.ButtonDown += OnExitGamePressed;

        SaveNewButton.ButtonDown += OnSaveNewButtonPressed;
        BackButton.ButtonDown += OnSaveBackButtonDown;
    }

    private void OnSaveNewButtonPressed()
    {
        var time = Time.GetTimeDictFromSystem();
        string saveName = string.Format("{0:00}-{1:00}-{2:00}",
                                        time["hour"],
                                        time["minute"],
                                        time["second"]);
        GlobalManager.GetInfos().SavePlayerInfo(saveName);
        LoadSaveList();
    }
    private void OnBackToGamePressed()
    {
        HidePauseMenu();
    }
    private void OnQuickSavePressed()
    {
        GlobalManager.GetInfos().SavePlayerInfo(PathManager.AUTO_SAVE_FILE_NAME);
        LoadSaveList();
    }
    private void OnExitGamePressed()
    {
        GetTree().Quit();
    }
    public void ShowPauseMenu()
    {
        GetTree().Paused = true;
        Visible = true;
        LoadSaveList();
    }

    public void HidePauseMenu()
    {
        GetTree().Paused = false;
        Visible = false;
    }

    public async void OnSaveBackButtonDown()
    {
        var tween = CreateTween();
        tween.TweenProperty(SavePanel, "size", new Vector2(500, 1), 0.2)
        .SetTrans(Tween.TransitionType.Quart).SetEase(Tween.EaseType.Out);
        await ToSignal(tween, Tween.SignalName.Finished);
        SavePanel.Visible = false;
    }
    public async void OnSaveListButtonDown()
    {
        GD.Print("加载保存列表");
        SavePanel.Visible = true;
        var tween = CreateTween();
        tween.TweenProperty(SavePanel, "size", new Vector2(500, 700), 0.2)
        .SetTrans(Tween.TransitionType.Quart).SetEase(Tween.EaseType.Out);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    public void LoadSaveList()
    {
        
        for(int i = 0; i < SaveContainer.GetChildCount(); i++)
        {
            SaveContainer.GetChild(i).QueueFree();
        }
        

        var dir = DirAccess.Open(PathManager.SAVE_PATH);
        if(dir == null){
            return;
        }
       
        dir.ListDirBegin();
        
        string fileName = dir.GetNext();
        GD.Print("next while:"+fileName);
        while(!string.IsNullOrEmpty(fileName))
        {
            var saveItem = GD.Load<PlayerInfo>(PathManager.GetSaveFilePathByFileName(fileName));
            if(saveItem == null){
                GD.PrintErr("无法加载保存文件: " + fileName);
                continue;
            }
            var saveLine = GameNormalScene.SaveScene.Instantiate<SaveData>();
            saveLine.playerInfo = saveItem;
            saveLine.setFileNameAndTime(fileName, saveItem.nowTime);
            SaveContainer.AddChild(saveLine);
            fileName = dir.GetNext();
        }
    }
    
}
