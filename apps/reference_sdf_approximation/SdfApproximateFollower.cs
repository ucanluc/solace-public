using Godot;
using Solace.addons.solace_core_plugin.lib.sdf_approximation;
using Solace.addons.solace_core_plugin.lib.utilities;

namespace Solace.apps.reference_sdf_approximation;

public partial class SdfApproximateFollower : CharacterBody3D
{
    [Export] private float _lerpSpeed = 10f;
    [Export] private Node3D _directionReference;

    private Vector3 _currentDirection = Vector3.Zero;


    private float _currentSpeed = 4.5f;
    private float _raycastDistance = 10;
    private float _lastHeightRatio = 0.5f;

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
        UpdateApproximatorSnapshot();

        var inputLocalSpace = GetLocalSpaceInput();
        var upFacingBasis = GetBasisWithDesiredRotation();

        var inputGlobalSpace = upFacingBasis * inputLocalSpace;
        GlobalBasis = upFacingBasis;

        _currentDirection = _currentDirection.Lerp(inputGlobalSpace, (float)(_lerpSpeed * delta));

        var velocity = _currentDirection * _currentSpeed;
        Velocity = velocity;

        MoveAndSlide();
    }

    private Basis GetBasisWithDesiredRotation()
    {
        var cameraFacingBasis = GlobalBasis.AlignSameLocalVectorGlobally
        (
            Vector3.Forward,
            _directionReference.GlobalBasis
        );

        var upFacingBasis = cameraFacingBasis.AlignLocalVectorToGlobalVector
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

        var currentHeightRatio = _approximator.Snapshot.HeightRatio;
        if (Mathf.IsZeroApprox(verticalDirection))
        {
            verticalDirection = _lastHeightRatio - currentHeightRatio;
        }
        else
        {
            _lastHeightRatio = currentHeightRatio;
        }

        var inputLocalSpace = new Vector3(inputDirection.X, verticalDirection, -inputDirection.Y).Normalized();
        return inputLocalSpace;
    }

    private void UpdateApproximatorSnapshot()
    {
        _approximator.Snapshot.NormalFitVector = GlobalBasis * (Vector3.Up * 0.5f);
        var spaceState = GetWorld3D().DirectSpaceState;
        _approximator.UpdateSnapshot(spaceState, GlobalPosition, _raycastDistance);
    }
}