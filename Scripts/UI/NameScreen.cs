using Godot;
using System;

public partial class NameScreen : Control
{
    [Export]
    public LineEdit StartGame;
    [Export]
    public Button Confirm;
    public override void _Ready()
    {
        base._Ready();
        Confirm.ButtonDown += OnConfirmPressed;
    }
    private void OnConfirmPressed()
    {
        var playerInfo = new PlayerInfo();
        PlayerInit(playerInfo);
    }

    public void PlayerInit(PlayerInfo player)
    {
        var playerName = StartGame.Text;
        if (string.IsNullOrEmpty(playerName))
        {
            GD.PrintErr("玩家名称不能为空");
            return;
        }
        player.PlayerName = playerName;
        player.Location = GameNormalScene.Site1Scene;
        player.HandMax = 100;

        var folderPath = PathManager.SAVE_PATH;
        var savePath = PathManager.GetSaveFilePathByName(playerName);

        PathManager.CreateFolder(folderPath);
        player.FolderPath = folderPath;
        long seconds = (long)Time.GetUnixTimeFromSystem();
        long milliseconds = (long)Time.GetTicksMsec();
        player.nowTime = seconds * 1000 + (milliseconds % 1000);
        GD.Print("savePath:"+savePath);
        // 尝试保存文件并检查结果
        Error saveResult = ResourceSaver.Save(player, savePath);
        if (saveResult != Error.Ok)
        {
            GD.PrintErr("保存文件失败: " + saveResult.ToString() + ", 路径: " + savePath);
            return;
        }
        GD.Print("保存文件成功: " + savePath);
        // 加载玩家信息
        GlobalManager.GetInfos().LoadPlayerInfo(playerName);
    }
}
