using Godot;
using System;

public partial class ShopItemCard : Card
{
    //商店卡牌处理对应的对话
    public const string LACK_OF_MONEY = "lackOfMoney";
    public const string WORTHLESS = "worthless";
    public const string DEAL = "deal";

    [Export]
    public Label DescriptionLabel;
    [Export]
    public Label PriceLabel;
    [Export]
    public Label PriceLabel2;
    [Export]
    public Button BuyButton;

    public string ItemName;
    public int price;
    public float PriceRatio = 1.2f;

    public override void DrawCard()
    {
        DescriptionLabel.Text = cardInfo.BaseDescription;
        ItemName = cardInfo.CardShowName;

        price = Mathf.RoundToInt(int.Parse(cardInfo.BasePrice) * PriceRatio);
        PriceLabel2.Text = string.Format("{0}", price);
    }

    public override void OnButtonDown()
    {
        var infos = GlobalManager.GetInfos();
        if(infos.save.money >= price)
        {
            if(infos.AddNewCard(ItemName,infos.HandDeck,GetGlobalPosition())  !=null){
                infos.save.money -= price;
                infos.PlayerUpdate();
            }
        }
        else
        {
            GameEvents.RaiseOnShowDia(LACK_OF_MONEY);
            GlobalManager.GetNPCManager().customCounter++;
        }
    }
}
