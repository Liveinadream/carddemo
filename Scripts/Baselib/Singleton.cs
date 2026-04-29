using Godot;

/// <summary>
/// Godot 节点的单例基类。
/// 适用于需要挂载在 AutoLoad 或场景中的管理器。
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class Singleton<T> : Node where T : Node
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GD.PushError($"[Singleton] 尝试访问尚未初始化的单例: {typeof(T).Name}");
            }
            return _instance;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        if (_instance != null && _instance != this)
        {
            GD.PushWarning($"[Singleton] 检测到重复的单例实例: {typeof(T).Name}，正在移除旧实例。");
            _instance.QueueFree();
        }
        _instance = this as T;
    }

    protected override void Dispose(bool disposing)
    {
        if (_instance == this)
        {
            _instance = null;
        }
        base.Dispose(disposing);
    }
}