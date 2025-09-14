using Godot;
using System;

public partial class SiteCard : Card
{
    [Export]
    public Label siteLabel;
    public override void OnButtonDown()
    {
        base.OnButtonDown();
        GD.Print("点击了场地卡片");
        var path = GameNormalScene.GetSiteCardScenePath(cardInfo.RealCardName);
        foreach(Deck deck in  GlobalManager.GetDropableGroup())
        {
            deck.StorCard();
        }
        GlobalManager.GotoSceneByPath(path);
    }
    public override void DrawCard()
    {
        siteLabel.Text = cardInfo.BaseDescription;
        nameLabel.Text = cardInfo.CardShowName;
    }

}
