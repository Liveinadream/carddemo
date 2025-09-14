using DialogueManagerRuntime;
using Godot;
using System;

public partial class NpcCard : Card
{
    [Export]
    public TextureRect SiteImg;
    public override void DrawCard()
    {
        nameLabel.Text = cardInfo.CardShowName;
    }

    public override void OnButtonDown()
    {
        if(CardCurrentState == CardState.Hanging || FollowTarget == null){
            return;
        }

        GlobalManager.GetNPCManager().currentNpcCard = this;
        CardCurrentState = CardState.Hanging;
        ZIndex = 1000;
        GetParent().MoveChild(this,0);
        
        FollowTarget.Visible = false;
        var tween1 = CreateTween();
        tween1.TweenProperty(control, "size",
        new Vector2(1545, 317), 0.5f)
        .SetTrans(Tween.TransitionType.Expo)
        .SetEase(Tween.EaseType.Out);

        var tween2 = CreateTween();
        tween2.TweenProperty(SiteImg, "modulate",
        new Color(1, 1, 1, 1), 0.5f);

        var tween3 = CreateTween();
        tween3.TweenProperty(this, "global_position",
        GetParent<Control>().GlobalPosition,0.5)
        .SetTrans(Tween.TransitionType.Expo)
        .SetEase(Tween.EaseType.Out);

        pickButton.Visible = false;
        nameLabel.Visible = true;

        var dio  =DialogueManager.ShowDialogueBalloonScene(GameNormalScene.DialogueBalloon,
        ResourceLoader.Load<Resource>(GameNormalScene.DialogueNPC1)) as DialogueBalloon;
        dio.npcCard = this;
        GD.Print("开始对话");
    }

    public override void EscDialogue(){
        FollowTarget.Visible = true;
        CardCurrentState = CardState.Following;

        var tween1 = CreateTween();
        tween1.TweenProperty(control, "size",
        new Vector2(220, 317), 0.3f)
        .SetTrans(Tween.TransitionType.Expo)
        .SetEase(Tween.EaseType.Out);

        var tween2 = CreateTween();
        tween2.TweenProperty(SiteImg, "modulate",
        new Color(0.9f, 0.9f, 0.9f, 0.863f), 0.3f);

        pickButton.Visible = true;
        nameLabel.Visible = false;

        GlobalManager.GetNPCManager().currentNpcCard = null;
    }
}
