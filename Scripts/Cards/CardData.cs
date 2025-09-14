using Godot;

/*
 * 卡牌数据类
 * 用于存储卡牌的基本信息和属性
 * 
 * 主要属性:
 * - BaseCard: 基础卡牌名称
 * - Index: 卡牌索引
 * - BaseDisplay: 基础显示名称
 * - BaseCardType: 基础卡牌类型
 * - BaseDescription: 基础描述
 * - BasePrice: 基础价格
 * - BaseMaxStack: 基础最大堆叠数量
 * - SiteArea: 站点区域
 * - NpcSchedule: NPC计划
 * - FoodHP: 食物HP
 * 
 * 主要功能:
 * - 存储和管理卡牌的基本信息和属性
 * - 提供卡牌数据的访问接口
 * 
 * 使用方法:
 * 1. 创建CardData实例
 * 2. 设置属性值
 * 3. 使用属性值进行卡牌操作
 */

public partial class CardData : Resource
{
    [Export]
    public string RealCardName { get; set; }
    
    [Export]
    public int Index { get; set; }
    
    [Export]
    public string CardShowName { get; set; }
    
    [Export]
    public string BaseCardType { get; set; }
    
    [Export]
    public string BaseDescription { get; set; }
    
    [Export]
    public int Weight { get; set; } = 0;
    
    [Export]
    public string BasePrice { get; set; }
    
    [Export]
    public int BaseMaxStack { get; set; }
    
    [Export]
    public int SiteArea { get; set; }
    
    [Export]
    public int NpcSchedule { get; set; }
    
    [Export]
    public int FoodHP { get; set; }

    // 构造函数保持不变
    public CardData() {}

    public CardData(string baseCard, int index, string baseDisplay, string baseCardType, 
                   string baseDescription, int weight, string basePrice, int baseMaxStack, 
                   int siteArea, int npcSchedule, int foodHP)
    {
        RealCardName = baseCard;
        Index = index;
        CardShowName = baseDisplay;
        Weight = weight;
        BaseCardType = baseCardType;
        BaseDescription = baseDescription;
        BasePrice = basePrice;
        BaseMaxStack = baseMaxStack;
        SiteArea = siteArea;
        NpcSchedule = npcSchedule;
        FoodHP = foodHP;
    }

    public override string ToString()
    {
        return $"BaseCard: {RealCardName}, Index: {Index}, CardName: {CardShowName}, Weight: {Weight}, BaseCardType: {BaseCardType}, BaseDescription: {BaseDescription}, BasePrice: {BasePrice}, BaseMaxStack: {BaseMaxStack}, SiteArea: {SiteArea}, NpcSchedule: {NpcSchedule}, FoodHP: {FoodHP}";

    }
}