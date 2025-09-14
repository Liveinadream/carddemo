using Godot;
using System;

public partial class GetInButton : Button
{
    public override void _Ready()
    {
        base._Ready();
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
    private void OnMouseEntered()
    {
        GD.Print("鼠标进入了按钮");
        var tween = CreateTween();
        tween.TweenProperty(this, "modulete", new Color(1, 1, 1, 1), 0.2f);
    }
    private void OnMouseExited()
    {
        GD.Print("鼠标退出了按钮");
        var tween = CreateTween();
        tween.TweenProperty(this, "modulete", new Color(1, 1, 1, 0), 0.2f)
        .Finished+=() =>{};
    }
}
