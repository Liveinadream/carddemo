using Godot;
using System;

public partial class PathManager
{
    public const string AUTO_SAVE_FILE_NAME = "autoSave";
    public const string IMAGE_PATH = "res://CardImg/";
    public const string SAVE_PATH = "user://save/";
    public const string SAVE_FILE_TYPE = ".tres";
    public const string IMAGE_FILE_TYPE = ".png";

    public static void CreateFolder(string folderPath)
    {
        // 直接使用 DirAccess.MakeDirRecursive 来创建目录，无需先打开
        var result = DirAccess.MakeDirRecursiveAbsolute(folderPath);
        if (result != Error.Ok)
        {
            GD.PrintErr("创建文件夹失败: " + result.ToString());
        }
    }

    // 添加一个辅助方法来构建完整的保存路径
    public static string GetSaveFilePathByName(string playerName)
    {
         if(playerName.EndsWith(SAVE_FILE_TYPE)){
            return SAVE_PATH + playerName;
        }
        return SAVE_PATH + playerName + SAVE_FILE_TYPE;
    }

    public static string GetSaveFilePathByFileName(string fileName)
    {
        if(fileName.EndsWith(SAVE_FILE_TYPE)){
            return SAVE_PATH + fileName;
        }
        return SAVE_PATH + fileName + SAVE_FILE_TYPE;
    }

    // 添加一个辅助方法来构建玩家文件夹路径
    public static string GetPlayerFolderPath(string playerName)
    {
        return SAVE_PATH + playerName;
    }
}


