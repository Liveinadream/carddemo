using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class GlobalManager : Node
{

    // 单例实例
    public static GlobalManager Instance { get; private set; }

    // 全局场景引用
    public CanvasLayer VfSlayer { get; private set; }
    public Infos Infos { get; private set; }

    public PauseMenu PauseMenu { get; private set; }
    public NPCManager NPCManager { get; private set; }

    // 调试模式设置
    private bool _enableDebugChecks = true;

    public override void _Ready()
    {
        // 设置单例
        Instance = this;
        // 在调试模式下执行校验
        if (OS.IsDebugBuild() && _enableDebugChecks)
        {
            PerformDebugChecks();
        }

        // 查找并缓存全局场景
        VfSlayer = GetTree().Root.FindChild(GameNormalScene.GlobalVfsPlayer, true, false) as CanvasLayer;
        if (VfSlayer == null)
        {
            GD.PrintErr("Failed to find VFSLayer node");
        }

        Infos = GetTree().Root.FindChild(GameNormalScene.GlobalInfos, true, false) as Infos;
        if (Infos == null)
        {
            GD.PrintErr("Failed to find Infos node");
        }
        PauseMenu = GetTree().Root.FindChild(GameNormalScene.GlobalPauseMenu, true, false) as PauseMenu;
        if (PauseMenu == null)
        {
            GD.PrintErr("Failed to find PauseMenu node");
        }
        NPCManager = GetTree().Root.FindChild(GameNormalScene.GlobalNPCManager, true, false) as NPCManager;
        if (NPCManager == null)
        {
            GD.PrintErr("Failed to find NPCManager node");
        }

    }

    // 调试校验方法
    private static void PerformDebugChecks()
    {
        // 调用场景路径和UID数量校验
        GameNormalScene.CheckPathAndUidNum();

        // 可以在这里添加其他调试校验
        // 例如：检查关键节点是否存在、配置是否正确等
    }

    // 提供全局访问方法
    public static CanvasLayer GetVfSlayer()
    {
        return Instance.VfSlayer;
    }

    public static Infos GetInfos()
    {
        return Instance.Infos;
    }
    public static PauseMenu GetPauseMenu()
    {
        return Instance.PauseMenu;
    }

     public static void PrintNodeTree()
    {
        if (Instance == null)
        {
            GD.PrintErr("GlobalManager instance not found");
            return;
        }

        var rootNode = Instance.GetTree().Root;
        GD.Print("=== 节点树结构 ===");
        PrintNodeRecursive(rootNode, 0);
    }

    private static void PrintNodeRecursive(Node node, int depth)
    {
        // 按层级缩进
        string indent = new(' ', depth * 2);
        GD.Print($"{indent}- {node.Name} ({node.GetType().Name}) - Path: {node.GetPath()}");

        // 递归打印所有子节点
        foreach (Node child in node.GetChildren())
        {
            PrintNodeRecursive(child, depth + 1);
        }
    }
    // 获取卡片组节点
    public static Array<Deck> GetDeckGroup()
    {
        if (Instance == null)
        {
            GD.PrintErr("GlobalManager instance not found");
            return [];
        }
        var cardDeckGroup = Instance.GetTree().GetNodesInGroup(GroupManager.CARD_DECK).OfType<Deck>();
        return new Array<Deck>(cardDeckGroup);

    }
    //获取在中间的牌桌
    public static Deck GetMidDeck(){
        if (Instance == null)
        {
            GD.PrintErr("GlobalManager instance not found");
            return null;
        }
        var midDeck = Instance.GetTree().GetNodesInGroup(GroupManager.MID_DECK).OfType<Deck>();
        return midDeck.FirstOrDefault();
    }

    // 获取可放置卡片的区域节点
    public static Array<Deck> GetDropableGroup()
    {
        if (Instance == null)
        {
            GD.PrintErr("GlobalManager instance not found");
            return [];
        }
        var cardDropAbleGroup = Instance.GetTree().GetNodesInGroup(GroupManager.CARD_DROPABLE).OfType<Deck>();

        return new Array<Deck>(cardDropAbleGroup);
    }

    // 获取可保存卡片的区域节点
    public static Array<Deck> GetStoreGroup()
    {
        if (Instance == null)
        {
            GD.PrintErr("GlobalManager instance not found");
            return [];
        }
        var cardStoreGroup = Instance.GetTree().GetNodesInGroup(GroupManager.CARD_SAVEABLE_DECK).OfType<Deck>();

        return new Array<Deck>(cardStoreGroup);
    }

    // 通用的获取组节点方法
    public static Array<Node> GetNodesInGroup(string groupName)
    {
        if (Instance == null)
        {
            GD.PrintErr("GlobalManager instance not found");
            return [];
        }
        return Instance.GetTree().GetNodesInGroup(groupName);
    }

    // 通过UID加载场景
    public static void GotoSceneByUid(string uid)
    {

        if (Instance == null)
        {
            GD.PrintErr("GlobalManager instance not found");
            return;
        }
        var scene = ResourceLoader.Load<PackedScene>(uid);
        if (scene != null)
        {
            Instance.GetTree().ChangeSceneToPacked(scene);
        }
        else
        {
            GD.PrintErr("无法加载场景: " + uid);
        }
    }

    // 通过路径加载场景
    public static void GotoSceneByPath(string path)
    {
        if (Instance == null)
        {
            GD.PrintErr("GlobalManager instance not found");
            return;
        }
        Instance.GetTree().ChangeSceneToFile(path);
    }
    // 添加全局访问方法
    public static NPCManager GetNPCManager()
    {
        return Instance.NPCManager;
    }

    // 场景导航方法
    public static void GotoSite1() => GotoSceneByUid(GameNormalScene.Site1SceneUid);
    public static void GotoSite2() => GotoSceneByUid(GameNormalScene.Site2SceneUid);
    public static void GotoStartScreen() => GotoSceneByUid(GameNormalScene.StartScreenSceneUid);
    public static void GotoNameScreen() => GotoSceneByUid(GameNormalScene.NameScreenSceneUid);

    
}