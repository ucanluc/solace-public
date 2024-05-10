using Godot;
using Solace.addons.solace_core_plugin.lib.sdf_approximation;

namespace Solace.apps.reference_insectoid;

public partial class InsectoidCharacterBody : CharacterBody3D
{
    [Export] private float _lerpSpeed = 10f;
    [Export] private Node3D _directionReference;
    [Export] private Node3D _skyMarker;
    [Export] private Node3D _groundMarker;
    [Export] private Node3D _positionMarker;

    private Vector3 _currentDirection = Vector3.Zero;


    private float _currentSpeed = 4.5f;
    private float raycastDistance = 10f;

    private readonly SdfApproximator _approximator = new(250, 0b1);


    public override void _PhysicsProcess(double delta)
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        _approximator.UpdateSnapshot(spaceState, GlobalPosition, raycastDistance);

        _groundMarker.Position = _approximator.Snapshot.GroundNormal;
        _skyMarker.Position = _approximator.Snapshot.SkyDirection;
        _positionMarker.GlobalPosition = _approximator.Snapshot.GroundPoint;

        var velocity = Velocity;

        var inputDirection = Input.GetVector("sc_move_left", "sc_move_right", "sc_move_backward", "sc_move_forward");
        var verticalDirection = Input.GetAxis("sc_modifier_careful", "sc_modifier_rush");
        var desiredDirection = _directionReference.Transform.Basis *
                               new Vector3(inputDirection.X, verticalDirection, -inputDirection.Y).Normalized();
        _currentDirection = _currentDirection.Lerp(desiredDirection, (float)(_lerpSpeed * delta));

        velocity.X = _currentDirection.X * _currentSpeed;
        velocity.Y = _currentDirection.Y * _currentSpeed;
        velocity.Z = _currentDirection.Z * _currentSpeed;


        Velocity = velocity;
        MoveAndSlide();
    }
}