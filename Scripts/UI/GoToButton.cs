using Godot;
using System;
using System.Linq;

public partial class GoToButton : Button
{
    [Export]
    public string ScenePath { get; set; } = ""; // 改为字符串类型

    public override void _Ready()
    {
        base._Ready();
        ButtonDown += () =>
        {
            var saveableDecks = GlobalManager.GetNodesInGroup(GroupManager.CARD_SAVEABLE_DECK).Cast<Deck>();
            foreach (var item in saveableDecks)
            {
                item.StorCard();
            }
            if (!string.IsNullOrEmpty(ScenePath))
            {
                // CallDeferred(nameof(SwitchScene), ScenePath);
                GetTree().ChangeSceneToFile(ScenePath);
            }
            else{
                GD.Print("ScenePath is null");
            }
        };
    }
   
    // private void SwitchScene(string path)
    // {
    //     // 执行场景切换
        
    // }
}
