using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class Main : Node2D
{
    [Export]
    public Deck Scene1 { get; set; }
    [Export]
    public Deck Scene2 { get; set; }

    [Export]
    public int MaxRandomItemNum { get; set; } = 0;
    [Export]
    public int MinRandomItemNum2 { get; set; } = 0;

    [Export]
    public Dictionary<string, int> SiteItem { get; set; }
    public Button generateButton;
    public override void _Ready()
    {
        base._Ready();
        generateButton = GetNode<Button>("Button");
        generateButton.Pressed += () =>
        {
            GenerateRandomCards();
        };
        GD.Print("Main load tscn ：" + Name);
        // GlobalManager.PrintNodeTree();
        GlobalManager.PrintNodeTree();
    }



    public void GenerateRandomCards()
    {
        GD.Print("GenerateRandomCards Button click");
        if (SiteItem == null || SiteItem.Count == 0)
        {
            GD.PrintErr("SiteItem is empty or null");
            return;
        }
    
        // 生成随机卡牌数量（范围：MinRandomItemNum2 到 MaxRandomItemNum）
        var random = new Random();
        int cardCount = random.Next(MinRandomItemNum2, MaxRandomItemNum + 1);
    
        // 准备权重总和
        int totalWeight = 0;
        foreach (var weight in SiteItem.Values)
        {
            totalWeight += weight;
        }
    
        // 生成指定数量的卡牌
        for (int i = 0; i < cardCount; i++)
        {
            // 根据权重随机选择卡牌名称
            string selectedCardName = SelectCardByWeight(random, totalWeight);
            if (selectedCardName != null)
            {
                var randomDeck = GetTree().GetNodesInGroup(GroupManager.CARD_DROPABLE)
                .PickRandom() as Deck;
                // 使用现有的 addNewCard 方法添加卡牌
                GlobalManager.GetInfos()
                .AddNewCard(selectedCardName, randomDeck,generateButton.GlobalPosition);
            }
        }
    }
    
    private string SelectCardByWeight(Random random, int totalWeight)
    {
        int randomValue = random.Next(0, totalWeight);
        int currentWeight = 0;
    
        foreach (var kvp in SiteItem)
        {
            currentWeight += kvp.Value;
            if (randomValue < currentWeight)
            {
                return kvp.Key;
            }
        }
    
        // 如果出现意外情况，返回第一个卡牌名称
        return SiteItem.Keys.FirstOrDefault();
    }

    public void getTotalWeight()
    {
        int totalWeight = 0;
        foreach (var weight in SiteItem.Values)
        {
            totalWeight += weight;
        }
    }
}
