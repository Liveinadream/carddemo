using Godot;
using System.Reflection;

/**
 * 游戏普通场景
 * 包含开始场景、名称场景、站点1场景、站点2场景
 */
public partial class GameNormalScene
{

    // 全局变量
    public const string GlobalVfsPlayer = "VfSlayer";
    public const string GlobalInfos = "Infos";
    public const string GlobalPauseMenu = "PauseMenu";
    public const string GlobalNPCManager = "NpcManager";

    //常用加载场景
    public static readonly PackedScene CardBackgroundScene = ResourceLoader.Load<PackedScene>("res://Scenes/Cards/card_background.tscn");
    public static readonly PackedScene CardScene = ResourceLoader.Load<PackedScene>("res://Scenes/Cards/card.tscn");
    public static readonly PackedScene SiteCardScene = ResourceLoader.Load<PackedScene>("res://Scenes/Cards/site_card.tscn");
    public static readonly PackedScene NPCCardScene = ResourceLoader.Load<PackedScene>("res://Scenes/Cards/npc_card.tscn");
    public static readonly PackedScene SaveScene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/save.tscn");
    public static readonly PackedScene ShopCardScene = ResourceLoader.Load<PackedScene>("res://Scenes/Cards/shop_card.tscn");
    public static readonly PackedScene ShopItemCardScene = ResourceLoader.Load<PackedScene>("res://Scenes/Cards/shop_item_card.tscn");
    
    // 对话场景s
    public const string DialogueBalloon = "res://Scenes/Dialogue/npc_balloon.tscn";
    public const string ShopBalloon = "res://Scenes/Dialogue/shop_balloon.tscn";
    public const string DialogueNPC1 = "res://Dialogue/dialogue.dialogue";
    public const string ShopNPC1 = "res://Dialogue/shop.dialogue";
    // 跳转场景路径
    public const string StartScreenScene = "res://Scenes/UI/start_screen.tscn";
    public const string NameScreenScene = "res://Scenes/UI/name_screen.tscn"; 
    public const string Site1Scene = "res://Scenes/Site/site1.tscn";
    public const string Site2Scene = "res://Scenes/Site/site2.tscn";

    // 使用UID引用的场景
    public const string StartScreenSceneUid = "uid://w6rb6r7fplrg";
    public const string NameScreenSceneUid = "uid://dbbnn2sot60du";
    public const string Site1SceneUid = "uid://d2wo0a1c8upir";
    public const string Site2SceneUid = "uid://bytgp5t2id57o";

    //根据名称加载对应场景
    public const string SiteCard = "res://Scenes/Site/";
    public const string SCENE_TYPE = ".tscn";

    public static string GetSiteCardScenePath(string name)
    {
        return SiteCard + name + SCENE_TYPE;
    }
    public static void CheckPathAndUidNum()
    {
        // 获取GameNormalScene类的所有字段
        FieldInfo[] fields = typeof(GameNormalScene).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        int pathCount = 0;
        int uidCount = 0;

        // 统计路径和UID常量的数量
        foreach (FieldInfo field in fields)
        {
            if (field.IsStatic && field.IsPublic && field.FieldType == typeof(string))
            {
                if (field.Name.EndsWith("Scene"))
                {
                    pathCount++;
                }
                else if (field.Name.EndsWith("SceneUid"))
                {
                    uidCount++;
                }
            }
        }

        // 检查数量是否匹配
        if (pathCount != uidCount)
        {
            GD.PrintErr("路径和UID数量不匹配");
        }
    }
}
