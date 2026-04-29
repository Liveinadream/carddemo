using Godot;
using System;
using System.Runtime.CompilerServices;

/* 日志级别 */
public enum ZLogLevel
{
    Debug,
    Info,
    Warn,
    Error
}

/* 轻量日志入口：目前统一转发到 Godot 的 GD.Print / GD.PushWarning / GD.PushError，后续可以在 Log(...) 内扩展写文件/上报等能力 */
public static class ZLog
{
    /* 打印调试日志
     * message: 日志内容
     * tag: 可选标签（用于筛选/分组）
     * context: 可选上下文（通常传 this，会打印类型与实例 ID）
     * exception: 可选异常（会打印异常类型与 Message）
     * data: 可选附加数据（会调用 ToString）
     * memberName/filePath/lineNumber: 调用方信息（默认由编译器自动填充）
     */
    public static void Debug(
        string message,
        string tag = null,
        GodotObject context = null,
        Exception exception = null,
        object data = null,
        [CallerMemberName] string memberName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int lineNumber = 0
    ) => Log(ZLogLevel.Debug, message, tag, context, exception, data, memberName, filePath, lineNumber);

    /* 打印信息日志（参数含义同 Debug） */
    public static void Info(
        string message,
        string tag = null,
        GodotObject context = null,
        Exception exception = null,
        object data = null,
        [CallerMemberName] string memberName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int lineNumber = 0
    ) => Log(ZLogLevel.Info, message, tag, context, exception, data, memberName, filePath, lineNumber);

    /* 打印警告日志（参数含义同 Debug） */
    public static void Warn(
        string message,
        string tag = null,
        GodotObject context = null,
        Exception exception = null,
        object data = null,
        [CallerMemberName] string memberName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int lineNumber = 0
    ) => Log(ZLogLevel.Warn, message, tag, context, exception, data, memberName, filePath, lineNumber);

    /* 打印错误日志（参数含义同 Debug） */
    public static void Error(
        string message,
        string tag = null,
        GodotObject context = null,
        Exception exception = null,
        object data = null,
        [CallerMemberName] string memberName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int lineNumber = 0
    ) => Log(ZLogLevel.Error, message, tag, context, exception, data, memberName, filePath, lineNumber);

    /* 字符串格式化版本：format + args，会转成单条 message（避免可选参数重载歧义） */
    public static void D(string format, params object[] args) => Debug(Format(format, args));
    public static void I(string format, params object[] args) => Info(Format(format, args));
    public static void W(string format, params object[] args) => Warn(Format(format, args));
    public static void E(string format, params object[] args) => Error(Format(format, args));

    /* 日志主入口：后续要写文件/分流/过滤时，优先在这里扩展 */
    public static void Log(
        ZLogLevel level,
        string message,
        string tag = null,
        GodotObject context = null,
        Exception exception = null,
        object data = null,
        string memberName = null,
        string filePath = null,
        int lineNumber = 0
    )
    {
        string line = BuildLine(level, message, tag, context, exception, data, memberName, filePath, lineNumber);

        switch (level)
        {
            case ZLogLevel.Warn:
                GD.PushWarning(line);
                break;
            case ZLogLevel.Error:
                GD.PushError(line);
                break;
            default:
                if(GameInfoManager.IsDebug){
                    GD.Print(line);
                }
                break;
        }
    }

    /* 拼装一行日志文本 */
    private static string BuildLine(
        ZLogLevel level,
        string message,
        string tag,
        GodotObject context,
        Exception exception,
        object data,
        string memberName,
        string filePath,
        int lineNumber
    )
    {
        string safeMessage = message ?? string.Empty;
        string levelText = level.ToString().ToUpperInvariant();
        ulong ms = Time.GetTicksMsec();

        string tagPart = string.IsNullOrWhiteSpace(tag) ? null : $"[{tag}]";
        string contextPart = context == null ? null : $"[{context.GetType().Name}:{context.GetInstanceId()}]";
        string callerPart = BuildCallerPart(memberName, filePath, lineNumber);

        string dataPart = data == null ? null : $" data={data}";
        string exceptionPart = exception == null ? null : $" ex={exception.GetType().Name}: {exception.Message}";

        return $"{ms} [{levelText}]{tagPart}{contextPart}{callerPart} {safeMessage}{dataPart}{exceptionPart}";
    }

    /* 拼装调用方信息 */
    private static string BuildCallerPart(string memberName, string filePath, int lineNumber)
    {
        if (string.IsNullOrWhiteSpace(memberName) && string.IsNullOrWhiteSpace(filePath) && lineNumber <= 0)
        {
            return null;
        }

        string fileName = string.IsNullOrWhiteSpace(filePath) ? null : System.IO.Path.GetFileName(filePath);
        if (string.IsNullOrWhiteSpace(fileName) && string.IsNullOrWhiteSpace(memberName) && lineNumber <= 0)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(fileName) && lineNumber > 0)
        {
            return $"[{fileName}:{lineNumber} {memberName}]";
        }

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            return $"[{fileName} {memberName}]";
        }

        if (lineNumber > 0)
        {
            return $"[{memberName}:{lineNumber}]";
        }

        return $"[{memberName}]";
    }

    /* 安全格式化：格式串不合法时返回原字符串 */
    private static string Format(string format, object[] args)
    {
        if (string.IsNullOrEmpty(format))
        {
            return string.Empty;
        }

        if (args == null || args.Length == 0)
        {
            return format;
        }

        try
        {
            return string.Format(format, args);
        }
        catch
        {
            return format;
        }
    }
}
