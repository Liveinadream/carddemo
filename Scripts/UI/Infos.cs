using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Infos : CanvasLayer
{
    [Export]
    public int MaxRandomItemNum { get; set; } = 0;
    [Export]
    public int MinRandomItemNum2 { get; set; } = 0;

    [Export]
    public Control FollowTarget { get; set; }
    [Export]
    public Dictionary<string, int> SiteItem { get; set; }

    // var var:Player
    public Dictionary<string,PackedScene> Saves { get; set; }

    [Export]
    public Deck HandDeck { get; set; }
    [Export]
    public Label MoneyLabel { get; set; }

    public PlayerInfo save;
    string playerInfoPath;
    public override void _Ready()
    {
        base._Ready();
    }
    public Card AddNewCard(string cardName, Deck cardDeck){
        return AddNewCard(cardName, cardDeck, ((Deck)GetTree().GetFirstNodeInGroup("cardDeck")).GlobalPosition);
    }
    public Card AddNewCard(string cardName,Deck cardDeck,Vector2 followTaget)
    {
        var cardClass = CardManager.Instance.GetCardDataByName(cardName);
        if(cardClass == null){
            return null;
        }
        Card cardToAdd;
        if (string.Equals(cardClass.BaseCardType,"site")){
            cardToAdd = GameNormalScene.SiteCardScene.Instantiate<SiteCard>();
        }
        else if (string.Equals(cardClass.BaseCardType,"npc")){
            cardToAdd = GameNormalScene.NPCCardScene.Instantiate<Card>();
        }
        else if (string.Equals(cardClass.BaseCardType,"shop")){
            cardToAdd = GameNormalScene.ShopCardScene.Instantiate<ShopCard>();
        }
        else{
            cardToAdd = GameNormalScene.CardScene.Instantiate<Card>();
        }
        cardToAdd.InitCard(cardClass);
        cardToAdd.SetGlobalPosition(followTaget);
        cardToAdd.ZIndex = 100;
        cardDeck.AddCard(cardToAdd);
        return cardToAdd;
    }

    public void GenerateRandomCards(Deck cardDeck)
    {
        // 验证参数
        if (cardDeck == null)
        {
            GD.PrintErr("cardDeck cannot be null");
            return;
        }
    
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
                // 使用现有的 addNewCard 方法添加卡牌
                AddNewCard(selectedCardName, cardDeck);
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

    public void SavePlayerInfo(string newSavePath){
        var deckGroup = GlobalManager.GetStoreGroup();
        foreach(Deck deck in deckGroup){
            deck.StorCard();
        }
        string path = save.FolderPath + newSavePath +PathManager.SAVE_FILE_TYPE;
        ResourceSaver.Save(save, path);
        GD.Print("保存成功到："+path);
    }

    public void LoadPlayerInfo()
    {
        LoadPlayerInfo(PathManager.AUTO_SAVE_FILE_NAME);
    }

    public void PlayerUpdate(){
        MoneyLabel.Text = "$" + save.money;
    }

    public void LoadPlayerInfo(string playerName)
    {
        // 使用 PathManger 辅助方法构建正确的保存路径
        string path = PathManager.GetSaveFilePathByName(playerName);
        playerInfoPath = path;

        // 检查文件是否存在
        if (!FileAccess.FileExists(path))
        {
            GD.PrintErr("无法找到玩家信息文件: " + path);
            return;
        }    

        // 加载玩家信息
        save = ResourceLoader.Load<PlayerInfo>(path);
        if (save == null)
        {
            GD.PrintErr("加载玩家信息失败: " + path);
            return;
        }

        HandDeck.maxWeight = save.HandMax;
        GetTree().ChangeSceneToFile(save.Location);
        // 加载卡牌
        HandDeck.LoadCards();
        Visible = true;
        PlayerUpdate();
    }
}