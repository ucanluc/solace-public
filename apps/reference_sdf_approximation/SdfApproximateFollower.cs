using Godot;
using Solace.addons.solace_core_plugin.lib.sdf_approximation;

namespace Solace.apps.reference_sdf_approximation;

public partial class SdfApproximateFollower : CharacterBody3D
{
    [Export] private float _lerpSpeed = 10f;
    [Export] private Node3D _directionReference;
    [Export] private Node3D _skyPositionMarker;
    [Export] private Node3D _groundNormalMarker;
    [Export] private Node3D _groundPositionMarker;
    [Export] private Node3D _surfaceProjectionMarker;
    [Export] private Node3D _roofProjectionMarker;
    [Export] private Node3D _skyDirectionMarker;
    [Export] private Node3D _groundDirectionMarker;

    private Vector3 _currentDirection = Vector3.Zero;


    private float _currentSpeed = 4.5f;
    private float raycastDistance = 10f;

    private readonly SdfApproximator _approximator = new(250, 0b1);


    public override void _Ready()
    {
        base._Ready();
        _approximator.Snapshot.DrawDebugLines = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        _approximator.UpdateSnapshot(spaceState, GlobalPosition, raycastDistance);

        _groundNormalMarker.GlobalPosition = GlobalPosition + _approximator.Snapshot.GroundNormal;

        _groundPositionMarker.GlobalPosition = _approximator.Snapshot.GroundPosition;
        _groundDirectionMarker.GlobalPosition = GlobalPosition + _approximator.Snapshot.GroundDirection;
        _surfaceProjectionMarker.GlobalPosition = GlobalPosition + ((GlobalBasis * Vector3.Down) * _approximator
            .Snapshot
            .DistanceToGround);

        _skyPositionMarker.GlobalPosition = _approximator.Snapshot.SkyPosition;
        _skyDirectionMarker.GlobalPosition = GlobalPosition + _approximator.Snapshot.SkyDirection;
        _roofProjectionMarker.GlobalPosition = GlobalPosition + ((GlobalBasis * Vector3.Up) * _approximator.Snapshot
            .DistanceToSky);


        var velocity = Velocity;


        var inputDirection = Input.GetVector("sc_move_left", "sc_move_right", "sc_move_backward", "sc_move_forward");
        var verticalDirection = Input.GetAxis("sc_modifier_careful", "sc_modifier_rush");
        var inputLocalSpace = new Vector3(inputDirection.X, verticalDirection, -inputDirection.Y).Normalized();


        var currentForward = (GlobalBasis * Vector3.Forward).Normalized();
        var desiredForward = (_directionReference.GlobalBasis * Vector3.Forward).Normalized();
        var rotationAxisForForward = currentForward.Cross(desiredForward).Normalized();
        if (rotationAxisForForward.IsZeroApprox())
        {
            rotationAxisForForward = Vector3.Up;
        }

        var angleDiff = currentForward.SignedAngleTo(desiredForward, rotationAxisForForward);
        var referenceAdjustedBasis = GlobalBasis.Rotated(rotationAxisForForward, angleDiff);


        // rotate from the current 'up' direction, to the desired 'up' direction; as found by the sdf approximator.
        var currentUp = (referenceAdjustedBasis * Vector3.Up).Normalized();
        var desiredUp = _approximator.Snapshot.GroundNormal;
        var rotationAxisForUp = currentUp.Cross(desiredUp).Normalized();
        if (rotationAxisForUp.IsZeroApprox())
        {
            rotationAxisForUp = Vector3.Forward;
        }

        var angleDifferenceForUp = currentUp.SignedAngleTo(desiredUp, rotationAxisForUp);
        var desiredBasisForUp = referenceAdjustedBasis.Rotated(rotationAxisForUp, angleDifferenceForUp);


        var inputGlobalSpace = desiredBasisForUp * inputLocalSpace;
        GlobalBasis = desiredBasisForUp;

        _currentDirection = _currentDirection.Lerp(inputGlobalSpace, (float)(_lerpSpeed * delta));

        velocity.X = _currentDirection.X * _currentSpeed;
        velocity.Y = _currentDirection.Y * _currentSpeed;
        velocity.Z = _currentDirection.Z * _currentSpeed;


        Velocity = velocity;

        MoveAndSlide();
    }
}