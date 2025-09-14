using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Linq;

public partial class Deck : Panel
{
    [Export]
    public Array<string> defaultCards;
    [Export]
    public Array<PackedScene> CardsSaved { get; set; } = new ();
    [Export]
    public Control cardDeck;
    [Export]
    public HBoxContainer cardPoiDeck;
    public int currentWeight;
    [Export]
    public Label weightLabel;
    [Export]
    public int maxWeight = 100;
    [Export]
    public ProgressBar progressBar;
    public override void _Ready()
    {

         if(IsInGroup(GroupManager.CARD_SAVEABLE_DECK))
        {
            LoadCards();
        }
        else
        {
           LoadDefaultCards();
        }
        progressBar.MaxValue = maxWeight;
    }
    bool isSort = false;

    public override void _Process(double delta)
    {
        if(cardDeck == null){
            return;
        }
        if(cardDeck.GetChildCount() > 0 && !isSort){
            Array<Card> children = new (cardDeck.GetChildren().Where(e => e is Card).Cast<Card>());
            SortNodeByPosition(children);
        }
    }
    private void LoadDefaultCards()
    {
        if(defaultCards == null || defaultCards.Count == 0){
            return;
        }
        foreach(var cardName in defaultCards){
            GlobalManager.GetInfos().AddNewCard(cardName,this);
        }
    }

    private void SortNodeByPosition(Array<Card> children)
    {   
        GD.Print("进行排序");
        //判断当前牌桌是否有卡牌处于Hanging 状态
        foreach(Card card in children){
            if(card.CardCurrentState == CardState.Hanging){
                GD.Print("有牌在悬停");
                return;
            }
        }

        var list = children.OrderBy(e => e.Position.X).ToList();
        
        for(int i = 0; i < list.Count; i++){
            if(list[i].CardCurrentState != CardState.Hanging){
                GD.Print("第"+i+"个牌的位置为"+list[i].cardInfo.CardShowName);
                list[i].ZIndex = i;
                cardDeck.MoveChild(list[i], i);
            }
        }
        isSort = true;
    }

    public virtual void AddCard(Card cardToAdd)
    {
        GD.Print("添加了名为"+cardToAdd.cardInfo.CardShowName+"的牌,卡牌的数量为："+cardToAdd.num);
        // StackTrace stackTrace = new();
        // GD.Print("调用堆栈：\n" + stackTrace.ToString());
        if((currentWeight + cardToAdd.num) <= maxWeight){
            if(CardIsStacked(cardToAdd)){
                return;
            }
        }
        else{
            GD.Print("超重了");
            return;
        }
        var index = cardToAdd.ZIndex;
        Control cardBackground = GameNormalScene.CardBackgroundScene.Instantiate() as Control;
        cardPoiDeck.AddChild(cardBackground);

        if(index <= cardPoiDeck.GetChildCount()){
            cardPoiDeck.MoveChild(cardBackground, index);
        }
        else{
            cardPoiDeck.MoveChild(cardBackground,-1);
        }

        var globalPoi = cardToAdd.GlobalPosition;

        cardToAdd.GetParent()?.RemoveChild(cardToAdd);
        cardDeck.AddChild(cardToAdd);
        cardToAdd.GlobalPosition = globalPoi;
        cardToAdd.FollowTarget = cardBackground;
        cardToAdd.SetNum(cardToAdd.num);
        cardToAdd.preDeck = this;
        cardToAdd.CardCurrentState = CardState.Following;
        UpdateWeight();
    }

    //判断是否已经有了相同的牌
    private bool CardIsStacked(Card cardToAdd)
    {

        if(cardDeck == null){
            return false;
        }
        foreach(var child in cardDeck.GetChildren()){
            if(child is Card card && card.CardCurrentState == CardState.Following){
                GD.Print(card.cardInfo.RealCardName +" anther "+cardToAdd.cardInfo.RealCardName);
                if(string.Equals(card.cardInfo.RealCardName,cardToAdd.cardInfo.RealCardName)){
                    FakerCardMove(card,cardToAdd);
                    return true;
                }
            }
        }
        return false;
    }

    private void FakerCardMove(Card card, Card cardToAdd)
    {
        cardToAdd.CardCurrentState = CardState.Faker;
        cardToAdd.ZIndex = 1000;

        cardToAdd.GetParent()?.RemoveChild(cardToAdd);
        GlobalManager.GetVfSlayer().AddChild(cardToAdd);
        cardToAdd.SetGlobalPosition(GetGlobalMousePosition() -new Vector2(125,100));
        var tween = CreateTween();
        tween.TweenProperty(cardToAdd, "global_position", card.GetGlobalPosition(), 0.2f)
        .SetTrans(Tween.TransitionType.Back)
        .SetEase(Tween.EaseType.Out).Finished += () =>
        {
            cardToAdd.QueueFree();
            var num = cardToAdd.num;
            while(num > 0){
                num--;
                card.AddNum();
            }
            UpdateWeight();
        };
    }

    public void UpdateWeight()
    {
        var nowWeight = 0;
        foreach(var child in cardDeck.GetChildren()){
            if(child is Card card && card.CardCurrentState == CardState.Following){
                nowWeight+=card.num;
            }
        }
        currentWeight = nowWeight;
        weightLabel.Text = currentWeight + "/" + maxWeight;
        progressBar.Value = 100*currentWeight/(float)maxWeight;
        GD.Print(Name+"当前的重量"+weightLabel.Text);
        isSort = false;
    }

    public void StorCard(){
        CardsSaved.Clear();
        foreach(Card child in cardDeck.GetChildren().Cast<Card>())
        {
            if(child.cardInfo == null){
                GD.PrintErr("卡牌信息为空");
                return;
            }


            var p =new PackedScene();
            var error =p.Pack(child);
            GD.Print("保存了名为"+child.cardInfo.CardShowName+"的牌,当前卡牌的数量为"+child.num+"，保存结果为:"+error.ToString());
            CardsSaved.Add(p);
        }
        DeckSaveCards saver = new()
            {
                CardsSaved = CardsSaved
            };
        var path = GetPath().ToString();
        var infos = GlobalManager.GetInfos();
        if(infos != null)
        {
            infos.save.Decks[path] = saver;
        }
    }

    public void LoadCards(){
        ClearChildren(cardPoiDeck);
        ClearChildren(cardDeck);
        string path = GetPath().ToString();

        var infos = GlobalManager.GetInfos();
        if(infos != null && infos.save != null && infos.save.Decks != null)
        {
            if(infos.save.Decks.TryGetValue(path, out var value))
            {
                // 添加类型检查
                if(value is DeckSaveCards saver)
                {
                    CardsSaved = saver.CardsSaved;
                    if(CardsSaved != null && CardsSaved.Count > 0)
                    {
                        foreach(PackedScene child in CardsSaved)
                        {
                            if(child != null && child.CanInstantiate())
                            {
                                AddCard(child.Instantiate<Card>());
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if(defaultCards == null){
                GD.Print("默认卡牌列表为空");
                return;
            }
            foreach(string cardName in defaultCards)
                {
                    GlobalManager.GetInfos().AddNewCard(cardName,this);
                }
        }
    }
    

    public void ClearChildren(Node node){
        if(node == null){
            return;
        }
        while (node.GetChildCount() > 0)
        {
            var child = node.GetChild(0);
            node.RemoveChild(child);
            child.QueueFree();
        }
    }

    public void ClearDeck(){
        ClearChildren(cardDeck);
        ClearChildren(cardPoiDeck);
    }
}
