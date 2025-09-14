using Godot;
using System;
using System.Linq;

public partial class ChangeAdleDeck : Deck
{
    [Export]
    public Button getAllButton;
    [Export]
    public Button compressedButton;
    [Export]
    public Button deleteButton;

    bool compressed = false;
    Vector2 compressedSize = new(130, 360);
    Vector2 normalSize = new(240, 360);

    public override void _Ready()
    {
        base._Ready();
        getAllButton.ButtonDown += GetAllCardDown;
        getAllButton.ButtonUp += GetAllCardUp;
        compressedButton.Toggled += Compressed;
        deleteButton.Pressed += DeleteCard;
        ChildEnteredTree += OnChildEnteredTree;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void Compressed(bool toggledOn)
    {
        GD.Print("压缩状态："+toggledOn);
        compressed = toggledOn;
        var cardPois = cardPoiDeck.GetChildren();
        foreach(Control cardPoi in cardPois.Cast<Control>())
        {
            if(compressed){
                cardPoi.CustomMinimumSize = compressedSize;
            }else{
                cardPoi.CustomMinimumSize =normalSize;
            }
        }
    }

    private void GetAllCardDown()
    {
        var cards = cardDeck.GetChildren();
        foreach(var card in cards){
            if(card is Card){
                (card as Card).OnAllButtonDown();
            }
        }
    }
    private void GetAllCardUp()
    {
        var cards = cardDeck.GetChildren();
        foreach(var card in cards){
            if(card is Card){
                (card as Card).OnAllButtonUp();
            }
        }
    }

    private void DeleteCard()
    {
        var cards = cardDeck.GetChildren();
        foreach(var card in cards){
            if(card is Card){
                (card as Card).QueueFree();
                UpdateWeight();
            }
        }
    }

    private void OnMouseExited()
    {
        var cardPois = cardDeck.GetChildren();
        foreach(Card cardPoi in cardPois.Cast<Card>())
        {
            if(cardPoi.CardCurrentState == CardState.Dragging){
                cardPoi.del = false;
            }
        }
    }

    private void OnMouseEntered()
    {
        var cardPois = cardDeck.GetChildren();
        foreach(Card cardPoi in cardPois.Cast<Card>())
        {
            if(cardPoi.CardCurrentState == CardState.Dragging){
                cardPoi.del = true;
            }
        }
    }

    private void OnChildEnteredTree(Node node)
    {
        if(compressed){
            if(node is Card){
                (node as Card).CustomMinimumSize =compressedSize;
            }
        }
    }
}
