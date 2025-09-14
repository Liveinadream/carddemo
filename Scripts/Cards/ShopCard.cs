using DialogueManagerRuntime;
using Godot;
using System;

public partial class ShopCard : Card
{

    [Export]
    public Label descriptionLabel;
    [Export]
    public Deck ShopCardDeck;
    float priceRatio = 0f;
    [Export]
    string[] priceItems;
    [Export]
    public Control ImgControl;
    [Export]
    public Button CloseButton;
    [Export]
    CanvasLayer deckp;
    [Export]
    public Button Button2;
    public Node dioScene;

    public override void DrawCard()
    {
        nameLabel.Text = cardInfo.CardShowName;
        descriptionLabel.Text = cardInfo.BaseDescription;
        priceRatio = cardInfo.Weight;
        priceItems = cardInfo.BasePrice.Split('!');
        GameEvents.OnShowDia += ShowDia;

        // ShopCardDeck.Connect("showDia", this, nameof(OnMouseEntered));
    }

    public override async void OnButtonDown()
    {
        GD.Print("购买");
        GlobalManager.GetNPCManager().currentNpcCard = this;
        pickButton.Visible = false;
        CardCurrentState = CardState.Hanging;

        deckp = GetParent<CanvasLayer>();
        var poi = GetGlobalPosition();
        deckp.RemoveChild(this);
        GlobalManager.GetVfSlayer().AddChild(this);
        SetGlobalPosition(poi);
        GD.Print("购买2");
        if (FollowTarget != null)
        {
            FollowTarget.Visible = false;
        }
        var tween1 = CreateTween();
        tween1.TweenProperty(ImgControl, "size", new Vector2(1545, 317), 0.5f)
        .SetTrans(Tween.TransitionType.Expo)
        .SetEase(Tween.EaseType.Out);

        CloseButton.Visible = true;
        nameLabel.Visible = true;

        var tween2 = CreateTween();
        tween2.TweenProperty(itemImg, "modulate", new Color(1, 1, 1, 1), 0.5f);

        var tween3 = CreateTween();
        tween3.TweenProperty(this, "global_position", new Vector2(355, 0), 0.5f)
        .SetTrans(Tween.TransitionType.Expo)
        .SetEase(Tween.EaseType.Out).Finished += () =>
        {
            GD.Print("购买3");
            ShopCardDeck.Visible = true;
            var tween4 = CreateTween();
            tween4.TweenProperty(ShopCardDeck, "size", new Vector2(1565, 360), 0.3f)
            .SetTrans(Tween.TransitionType.Expo)
            .SetEase(Tween.EaseType.Out).Finished += () =>
            {
                GD.Print("购买4");
                //隐藏中间的牌桌以放置商店需要卖的卡牌
                GlobalManager.GetMidDeck().Visible = false;
                var dio = DialogueManager.ShowDialogueBalloonScene(GameNormalScene.ShopBalloon,
                ResourceLoader.Load<Resource>(GameNormalScene.ShopNPC1)) as DialogueBalloon;
                dio.npcCard = this;
                GD.Print("开始对话");
            };
        };
        await ToSignal(tween3, "finished");
        GD.Print("购买5");
    }

    public void ShowDia(string diaName)
    {
        if (dioScene != null && IsInstanceValid(dioScene))
        {
            dioScene.QueueFree();
        }
        dioScene = DialogueManager.ShowDialogueBalloonScene(GameNormalScene.ShopBalloon,
        ResourceLoader.Load<Resource>(GameNormalScene.ShopNPC1), diaName) as DialogueBalloon;
    }

    public async void ShopAddCard()
    {
        GD.Print("购买6");
        foreach (var item in priceItems)
        {
            var cardToAdd = GameNormalScene.ShopItemCardScene.Instantiate<ShopItemCard>();
            cardToAdd.PriceRatio = priceRatio;
            // cardToAdd.Connect(ShopItemCard.SHOW_DIA, new Callable(this, nameof(ShowDia)));
            cardToAdd.InitCard(CardManager.Instance.GetCardDataByName(item));
            cardToAdd.SetGlobalPosition(ShopCardDeck.GlobalPosition + new Vector2(ShopCardDeck.Size.X, 0));

            //自定义牌桌添加部分
            var cardBackGroud = GameNormalScene.CardBackgroundScene.Instantiate<Control>();
            ShopCardDeck.cardPoiDeck.AddChild(cardBackGroud);
            var globalPosition = cardToAdd.GlobalPosition;
            if (cardToAdd.GetParent() != null)
            {
                cardToAdd.GetParent().RemoveChild(cardToAdd);
            }
            ShopCardDeck.cardDeck.AddChild(cardToAdd);
            cardToAdd.SetGlobalPosition(globalPosition);

            cardToAdd.FollowTarget = cardBackGroud;
            cardToAdd.CardCurrentState = CardState.Following;

            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        }
    }

    public async void Leave(){
        GD.Print("离开");
        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 0.3f);
        Visible = false;
        FollowTarget.Visible = false;
    }

    public async void OnEsc(){
        GD.Print("退出");
        var poi = GetGlobalPosition();
        GlobalManager.GetVfSlayer().RemoveChild(this);
        deckp.AddChild(this);
        SetGlobalPosition(poi);

        ShopCardDeck.ClearDeck();

        if (dioScene != null)
        {
            dioScene.QueueFree();
            dioScene = null;
        }
        GlobalManager.GetMidDeck().Visible = true;

        var tween = CreateTween();

        tween.TweenProperty(ShopCardDeck, "size", new Vector2(1565,1 ), 0.3f)
        .SetTrans(Tween.TransitionType.Expo)
        .SetEase(Tween.EaseType.Out);

        await ToSignal(tween, "finished");
        
        CardCurrentState = CardState.Following;
        Button2.Visible = false;
        FollowTarget.Visible = true;
        
        //隐藏npc
        var tween2 = CreateTween();
        tween2.TweenProperty(itemImg, "modulate", new Color(1, 1, 1, 0), 0.2f);

        //显示自己
        var tween1 = CreateTween();
        tween1.TweenProperty(control, "size", new Vector2(1545, 317), 0.5f)
        .SetTrans(Tween.TransitionType.Expo)
        .SetEase(Tween.EaseType.Out);

        ShopCardDeck.Visible = false;
        pickButton.Visible = true;
        nameLabel.Visible = false;
    }
}
