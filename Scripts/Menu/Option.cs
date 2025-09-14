using Godot;
using System;

public partial class Option : Button
{
    public override void _Ready()
    {
        ButtonDown += OnButtonDown;
    }
    private void OnButtonDown()
    {
        GlobalManager.GetPauseMenu().ShowPauseMenu();
    }
}
