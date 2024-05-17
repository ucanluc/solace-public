using Godot;
using Solace.addons.solace_core_plugin.lib.sdf_approximation;
using Solace.addons.solace_core_plugin.lib.utilities;

namespace Solace.apps.reference_sdf_approximation;

public partial class SdfApproximateFollower : CharacterBody3D
{
    [Export] private float _inputTurnStrength = 10f;
    [Export] private Node3D _directionReference;
    [Export] public Vector3 SkyPoint { get; private set; }

    private Vector3 _currentDirection = Vector3.Zero;


    private float _currentSpeed = 4.5f;
    private float _raycastDistance = 10;
    private float _lastHeight = 0.5f;

    private readonly SdfApproximator _approximator = new(100, 0b1);


    public override void _Ready()
    {
        base._Ready();
        InitialiseApproximatorSnapshot();
    }

    private void InitialiseApproximatorSnapshot()
    {
        _approximator.Snapshot.DrawDebugLines = true;
        _approximator.Snapshot.ObjectRadius = 1f;
    }

    public override void _PhysicsProcess(double delta)
    {
        var inputLocalSpace = GetLocalSpaceInput();
        UpdateApproximatorFit();
        UpdateApproximatorSnapshot();
        UpdateCorrectiveMovement(ref inputLocalSpace);


        var upFacingBasis = GetBasisWithDesiredRotation();
        var inputGlobalSpace = upFacingBasis * inputLocalSpace;
        GlobalBasis = upFacingBasis;


        _currentDirection = _currentDirection.Lerp(inputGlobalSpace, (float)(_inputTurnStrength * delta));

        var velocity = _currentDirection * _currentSpeed;
        Velocity = velocity;

        MoveAndSlide();
    }

    private void UpdateApproximatorFit()
    {
        var upDirection = GlobalBasis * (Vector3.Up);
        _approximator.Snapshot.WeightFitHitNormal = upDirection;
        _approximator.Snapshot.WeightFitDirection = Velocity * 0.25f;
        _approximator.Snapshot.WeightFitSurfaceNormal = Vector3.Up * 0.5f;
    }

    private void UpdateCorrectiveMovement(ref Vector3 inputLocalSpace)
    {
        var availableHeight = _approximator.Snapshot.DistanceToSky;
        var currentHeight = _approximator.Snapshot.DistanceToGround;
        var totalHeightAvailable = Mathf.Abs(currentHeight + availableHeight);
        if (Mathf.IsZeroApprox(inputLocalSpace.Y))
        {
            var desiredHeight = Mathf.Clamp(_lastHeight, 0, totalHeightAvailable);

            inputLocalSpace.Y = desiredHeight - currentHeight;
        }
        else
        {
            _lastHeight = currentHeight;
        }
    }

    private Basis GetBasisWithDesiredRotation()
    {
        var cameraFacingBasis = GlobalBasis.AlignSameLocalVectorGloballyWeighted
        (
            Vector3.Forward,
            _directionReference.GlobalBasis
        );

        var upFacingBasis = cameraFacingBasis.AlignLocalVectorToGlobalVectorWeighted
        (
            Vector3.Up,
            _approximator.Snapshot.SurfaceNormal
        );
        return upFacingBasis;
    }

    private Vector3 GetLocalSpaceInput()
    {
        var inputDirection = Input.GetVector("sc_move_left", "sc_move_right", "sc_move_backward", "sc_move_forward");
        var verticalDirection = Input.GetAxis("sc_modifier_careful", "sc_modifier_rush");
        var inputLocalSpace = new Vector3(inputDirection.X, verticalDirection, -inputDirection.Y).LimitLength(1f);
        return inputLocalSpace;
    }

    private void UpdateApproximatorSnapshot()
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        _approximator.UpdateSnapshot(spaceState, GlobalPosition, _raycastDistance);
        SkyPoint = _approximator.Snapshot.ProjectedSkyPosition;
    }
}