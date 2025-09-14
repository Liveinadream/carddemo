using DialogueManagerRuntime;
using Godot;
using System;
using System.Diagnostics;

public partial class ShopCardDeck : Deck
{
    float priceRatio = 1.0f;


    public override void _Ready()
    {
        base._Ready();
    }

    public override void AddCard(Card cardToAdd)
    {
        StackTrace stackTrace = new();
        GD.Print("调用堆栈：\n" + stackTrace.ToString());
        int price;
        try
        {
            price = int.Parse(cardToAdd.cardInfo.BasePrice);
        }
        catch (Exception e)
        {
            GD.PrintErr("解析价格失败: " +cardToAdd.cardInfo.CardShowName +" 错误为："+ e.Message);
            return;
        }
        if (price <= 0)
        {
           GameEvents.RaiseOnShowDia(ShopItemCard.WORTHLESS);
        }
        var Infos = GlobalManager.GetInfos();
        Infos.save.money += price*cardToAdd.num;
        Infos.PlayerUpdate();
        cardToAdd.QueueFree();
        GameEvents.RaiseOnShowDia(ShopItemCard.DEAL);
    }
}
