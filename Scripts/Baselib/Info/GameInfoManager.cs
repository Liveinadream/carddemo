using Godot;

public static class GameInfoManager
{
    /*
    * 获取应用程序名称。
    * 如果项目设置中未定义 "application/config/name"，则默认为 "Game"。
    */
    public static string AppName =>
        ProjectSettings.HasSetting("application/config/name")
            ? (string)ProjectSettings.GetSetting("application/config/name")
            : "Game";

    /*
    * 获取应用程序版本。
    * 如果项目设置中未定义 "application/config/version"，则默认为 "0.0.0"。
    */
    public static string AppVersion =>
        ProjectSettings.HasSetting("application/config/version")
            ? (string)ProjectSettings.GetSetting("application/config/version")
            : "0.0.0";

    /*
    * 获取 Godot 引擎版本。
    */
    public static string EngineVersion => (string)Engine.GetVersionInfo()["string"];
    public static bool ViewAllDebugInfo = false;
    /*
    * 判断当前是否为调试构建。
    */
    public static bool IsDebug => OS.IsDebugBuild()  && !ViewAllDebugInfo;

    /*
    * 获取构建类型（"Debug" 或 "Release"）。
    */
    public static string BuildFlavor => IsDebug ? "Debug" : "Release";

    /*
    * 获取完整的显示版本字符串，格式为 "AppName vAppVersion (BuildFlavor)"。
    */
    public static string DisplayVersion => $"{AppName} v{AppVersion} ({BuildFlavor})";
}
