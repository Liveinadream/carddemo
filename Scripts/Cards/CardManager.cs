using Godot;
using System.Collections.Generic;

/*
 * 卡牌管理器类
 * 负责加载卡牌数据、根据索引或名称获取卡牌数据
 * 
 * 主要功能:
 * - 加载卡牌数据文件
 * - 根据索引或名称查找卡牌数据
 * - 提供卡牌数据的访问接口
 * 
 * 使用方法:
 * 1. 创建CardManager实例
 * 2. 调用LoadCardsFromCsv方法加载卡牌数据
 * 3. 使用GetCardDataByIndex或GetCardDataByName方法获取卡牌数据
 */

[GlobalClass]
public partial class CardManager : Node
{
    private static CardManager _instance;
    public static CardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CardManager();
                _instance.Initialize();
            }
            return _instance;
        }
    }
    public void Initialize()
    {
        string csvPath = "res://Assets/cardsInfo.csv";
        _cardDataList = CardDataLoader.LoadCardsFromCsv(csvPath);
    }
    private List<CardData> _cardDataList;

    public override void _Ready()
    {
        Instance.Initialize();
    }

    // 根据索引获取卡牌数据
    public CardData GetCardDataByIndex(int index)
    {
        foreach (CardData cardData in _cardDataList)
        {
            if (cardData.Index == index)
                return cardData;
        }
        return null;
    }

    // 根据名称获取卡牌数据
    public CardData GetCardDataByName(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            GD.PrintErr($"name is null or empty:",new System.Diagnostics.StackTrace(true));
            return null;
        }
        GD.Print($"get name {name}");
        if(_cardDataList == null)
        {
            GD.Print($"_cardDataList is null");
            return null;
        }
        foreach (CardData cardData in _cardDataList)
        {
            if (string.Equals(cardData.RealCardName, name))
                return cardData;
        }
        GD.Print($"not found name {name}");
        return null;
    }
}