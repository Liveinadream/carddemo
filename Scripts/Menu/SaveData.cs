using Godot;
using System;
using System.IO;

public partial class SaveData : Panel
{
    [Export]
    public Button LoadGameButton;
    [Export]
    public Button DeleteGameButton;
    [Export]
    public Label SaveNameLabel;
    [Export]
    public Label SaveOtherInfo;

    public string SaveName;
    public PlayerInfo playerInfo;

    public string pureName;

    public override void _Ready()
    {
        LoadGameButton.ButtonDown += OnLoadGameButtonButtonDown;
        DeleteGameButton.ButtonDown += OnDeleteGameButtonButtonDown;
        if(playerInfo == null){
            GD.PrintErr("playerInfo 为空");
            return;
        }
    }

    public void OnLoadGameButtonButtonDown()
    {
        GlobalManager.GetPauseMenu().HidePauseMenu();
        GlobalManager.GetInfos().LoadPlayerInfo(pureName);
    }

    public void OnDeleteGameButtonButtonDown()
    {
        var path = PathManager.GetSaveFilePathByName(pureName);
        var dirAccess = DirAccess.Open(path.GetBaseDir());
        if(dirAccess != null && dirAccess.FileExists(path))
        {
           var error =  dirAccess.Remove(path);
           if(error == Error.Ok)
           {
               GD.Print("删除文件成功: " + path);
           }
           else
           {
               GD.PrintErr("删除文件失败: " + error.ToString() + ", 路径: " + path);
           }
        }
        else
        {
            GD.PrintErr("文件不存在: " + path);
        }
        // 刷新文件列表
        GlobalManager.GetPauseMenu().LoadSaveList();
    }

    public void setFileNameAndTime(string fileName,long time)
    {
        pureName = fileName;
        SaveNameLabel.Text = fileName;
        // 将 Unix 时间戳转换为 DateTime（假设是毫秒级）
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        .AddMilliseconds(time)
        .ToLocalTime();
    
        // 格式化日期时间（例如：2023-10-25 14:30:45）
        SaveOtherInfo.Text = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
