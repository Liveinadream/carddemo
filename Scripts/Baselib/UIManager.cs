using Godot;
using System.Collections.Generic;

//  <summary>
//  UI 管理器，用于控制 UI 面板的显示与关闭。
//  </summary>
public partial class UIManager : Singleton<UIManager>
{
    private Dictionary<string, Control> _openedPanels = new Dictionary<string, Control>();
    private Node _uiRoot;

    public override void _Ready()
    {
        base._Ready();
        _uiRoot = GetTree().Root.FindChild("VFSLayer", true, false)
                   ?? GetTree().Root.FindChild("VfSlayer", true, false)
                   ?? GetTree().Root.FindChild("Infos", true, false);
        if (_uiRoot == null) GD.PushWarning("[UIManager] 未找到可用的 UI 根节点 (VFSLayer/VfSlayer/Infos)。");
    }

    /// <summary>
    /// 打开指定的 UI 面板。
    /// </summary>
    public T OpenPanel<T>(string scenePath) where T : Control
    {
        if (_openedPanels.TryGetValue(scenePath, out var panel))
        {
            panel.Show();
            return panel as T;
        }

        if (ResLoader.Instance.InstantiateScene(scenePath) is T newPanel)
        {
            (_uiRoot ?? this).AddChild(newPanel);
            _openedPanels[scenePath] = newPanel;
            return newPanel;
        }

        GD.PushError($"[UIManager] 无法打开面板: {scenePath}");
        return null;
    }

    /// <summary>
    /// 关闭指定的 UI 面板。
    /// </summary>
    public void ClosePanel(string scenePath, bool destroy = false)
    {
        if (_openedPanels.TryGetValue(scenePath, out var panel))
        {
            if (destroy)
            {
                panel.QueueFree();
                _openedPanels.Remove(scenePath);
            }
            else
            {
                panel.Hide();
            }
        }
    }

    /// <summary>
    /// 获取已打开的面板。
    /// </summary>
    public T GetPanel<T>(string scenePath) where T : Control
    {
        if (_openedPanels.TryGetValue(scenePath, out var panel))
        {
            return panel as T;
        }
        return null;
    }

    /// <summary>
    /// 关闭所有面板。
    /// </summary>
    public void CloseAll(bool destroy = false)
    {
        foreach (var path in new List<string>(_openedPanels.Keys))
        {
            ClosePanel(path, destroy);
        }
    }
}
