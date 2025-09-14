using Godot;
using System;

public partial class Delete : Button
{
    bool isMouseInside = false;
    public override void _Process(double delta)
    {
        base._Process(delta);
        var mousePosition = GetGlobalMousePosition();
        var rect = GetGlobalRect();
        var currentlyInside = rect.HasPoint(mousePosition);
        
        if(currentlyInside != isMouseInside){
            isMouseInside = currentlyInside;
            if(isMouseInside){
                EmitSignal("mouse_entered");
            }
            else{
                EmitSignal("mouse_exited");
            }
        }
    }
}
