using Godot;
using System;

public partial class ToggleCamera2D : Camera2D
{
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("sc_action_knowledge"))
        {
            Enabled = true;
        }
    }
}