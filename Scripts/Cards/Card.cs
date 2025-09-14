using Godot;
using System;
using System.Drawing;

public partial class Card : Control
{

    public static int maxStackNum = 3;
    [Export]
    public CardState CardCurrentState { get; set; } = CardState.Following;
    [Export]
    public Control FollowTarget { get; set; }

    public Deck preDeck { get; set; }
    Deck whichDeckMouseIn;
    Vector2 velocity = Vector2.Zero;
    public const float damping = 0.35f;
    public int stiffness = 500;
    public Card dup;

    [Export]
    public Button pickButton;
    [Export]
    public Button allButton;
    public TextureRect textureRect;
    [Export]
    public Panel control;
    [Export]
    public ColorRect colorRect;
    [Export]
    public Label nameLabel;
    [Export]
    public TextureRect itemImg;
    [Export]
    public CardData cardInfo;
    
    [Export]
    public int num = 1;

    // 是否可以删除
    public bool del;

    public override void _Ready()
    {

        pickButton.ButtonDown += OnButtonDown;
        pickButton.ButtonUp += OnButtonUp;
        if(allButton != null){
            allButton.ButtonDown += OnAllButtonDown;
            allButton.ButtonUp += OnButtonUp;
        }
        
        DrawCard();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        switch (CardCurrentState)
        {
            case CardState.Dragging:
                var mousePos = GetGlobalMousePosition();
                Following(mousePos - Size / 2, delta);
                var nodes = GetTree().GetNodesInGroup(GroupManager.CARD_DROPABLE);
                foreach (var node in nodes)
                {
                    if (node is Deck deck && deck.GetRect().HasPoint(mousePos) && deck.Visible)
                    {
                        whichDeckMouseIn = deck;
                        break;
                    }
                }
                break;
            case CardState.Following:
                if (FollowTarget != null && IsInstanceValid(FollowTarget))
                {
                    Following(FollowTarget.GlobalPosition, (float)delta);
                }
                break;
            case CardState.VFS:
                Following(GetGlobalMousePosition() - Size / 2, delta);
                break;
        }
    }

    public void Following(Vector2 targetPos,double delta)
    {
        Vector2 displacement = targetPos - GlobalPosition;
        Vector2 force = displacement * stiffness;
        velocity += force * (float)delta;
        velocity *= 1 - damping;
        GlobalPosition += velocity * (float)delta;
    }

    public bool CardStack(Card cardToStack)
    {
        var stackNum = cardToStack.num;
        if(stackNum+num > maxStackNum){
            return false;
        }
        num = stackNum + cardToStack.num;
        DrawCard();
        GD.Print("卡牌进行了堆叠");
        return true;
    }

    public virtual void OnButtonDown()
    {
        if(CardCurrentState != CardState.Following){
            return;
        }
        var numTemp = num;
        num = 1;
        dup = (Card)Duplicate();
        GlobalManager.GetVfSlayer().AddChild(dup);
        dup.GlobalPosition = GlobalPosition;
        dup.CardCurrentState = CardState.VFS;
        CardCurrentState = CardState.Dragging;
        if (GetParent().GetParent() is not Deck)
        {
            return;
        }
        Deck deck = GetParent<Control>().GetParent<Deck>();
        deck.UpdateWeight();
        if(numTemp > 1){
            Card c = GlobalManager.GetInfos().AddNewCard(cardInfo.RealCardName,deck);
            c.FollowTarget?.QueueFree();
            if (FollowTarget != null && IsInstanceValid(FollowTarget))
            {
                c.FollowTarget = FollowTarget;
            }
            c.SetGlobalPosition(GlobalPosition);
            c.num = numTemp-1;
            c.DrawCard();
        }else{
            FollowTarget?.QueueFree();
            FollowTarget = null;
        }
        deck.UpdateWeight();
    }
    public void OnButtonUp()
    {
        if(CardCurrentState == CardState.Hanging){
            return;
        }
        dup?.QueueFree();
        if(del){
            QueueFree();
            GD.Print("delete");
            return;
        }
        if(whichDeckMouseIn != null){
            whichDeckMouseIn.AddCard(this);
        }
        else{
            if (preDeck!=null)
                preDeck.AddCard(this);
            else
                GD.Print("没有前置牌组,可能是点太快了");
        }
        CardCurrentState = CardState.Following;
    }

    public void OnAllButtonDown()
    {
        dup = (Card)Duplicate();
        GlobalManager.GetVfSlayer().AddChild(dup);
        dup.GlobalPosition = GlobalPosition;
        dup.CardCurrentState = CardState.VFS;
        CardCurrentState = CardState.Dragging;
        FollowTarget?.QueueFree();
        FollowTarget = null;
        
    }
    public void OnAllButtonUp()
    {
        if(CardCurrentState != CardState.Following){
            return;
        }
        if(whichDeckMouseIn != null){
            whichDeckMouseIn.AddCard(this);
        }
        else{
            preDeck.AddCard(this);
        }
        CardCurrentState = CardState.Following;
    }
    public void InitCard(CardData cardData)
    {
        cardInfo = cardData;
        CardCurrentState = CardState.Following;
    }

    public virtual void DrawCard()
    {
        if(cardInfo == null)
        {
            GD.PrintErr("卡牌数据为空");
            return;
        }
        string imagePath = PathManager.IMAGE_PATH + cardInfo.RealCardName + PathManager.IMAGE_FILE_TYPE;
        // 加载并设置纹理
        try
        {
            itemImg.Texture = GD.Load<Texture2D>(imagePath);
        }
        catch (Exception e)
        {
            GD.PrintErr("Failed to load texture: " + imagePath + ", Error: " + e.Message);
        }
        // 设置名称文本
        if(nameLabel != null)
            nameLabel.Text = cardInfo.CardShowName;
        if(allButton != null)
            allButton.Text = "*"+num;
    }

    public void AddNum()
    {
        num++;
        if(allButton != null)
            allButton.Text = "*"+num;
    }

    public void ResetNum()
    {
        num = 1;
        if(allButton != null)
            allButton.Text = "*"+num;
    }

    public void SetNum(int num)
    {
        this.num = num;
        if(allButton != null)
            allButton.Text = "*"+num;
    }

    public virtual void EscDialogue()
    {
        
    }
}

/*
 * 卡牌状态
 * Following 在牌桌状态
 * Dragging 拖拽状态
 * VFS 堆叠卡牌，拖拽虚拟状态
 * Faker 虚拟卡牌，用于堆叠
 */
public enum CardState
{
    Following,
    Dragging,
    VFS,
    Faker,
    Hanging
}

