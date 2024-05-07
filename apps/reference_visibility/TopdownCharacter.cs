using Godot;

namespace Solace.apps.visibility_2d;

public partial class TopdownCharacter : CharacterBody2D
{
    [Export] public int Speed { get; set; } = 400;

    private void GetInput()
    {
        LookAt(GetGlobalMousePosition()); // same as; GetGlobalMousePosition().AngleToPoint(Position);

        var inputDirection = Input.GetVector("sc_move_left", "sc_move_right", "sc_move_forward", "sc_move_backward");
        Velocity = inputDirection * Speed;
    }

    public override void _PhysicsProcess(double delta)
    {
        GetInput();
        MoveAndSlide();
    }
}