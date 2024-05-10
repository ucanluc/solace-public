using Godot;
using Solace.addons.solace_core_plugin.lib.sdf_approximation;

namespace Solace.apps.reference_insectoid;

public partial class InsectoidCharacterBody : CharacterBody3D
{
    [Export] public Node3D Head { get; set; }
    [Export] private float _lerpSpeed = 10f;

    private Vector3 _currentDirection = Vector3.Zero;
    private float _currentSpeed = 4.5f;
    private float raycastDistance = 10f;

    private readonly SdfApproximator _approximator = new(50, 0b1);


    public override void _PhysicsProcess(double delta)
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        _approximator.UpdateSnapshot(spaceState, GlobalPosition, raycastDistance);

        var velocity = Velocity;

        var inputDirection = Input.GetVector("sc_move_left", "sc_move_right", "sc_move_backward", "sc_move_forward");
        var desiredDirection = Head.Transform.Basis * new Vector3(inputDirection.X, 0, -inputDirection.Y).Normalized();
        _currentDirection = _currentDirection.Lerp(desiredDirection, (float)(_lerpSpeed * delta));

        velocity.X = _currentDirection.X * _currentSpeed;
        velocity.Z = _currentDirection.Z * _currentSpeed;

        Velocity = velocity;
        MoveAndSlide();
    }
}