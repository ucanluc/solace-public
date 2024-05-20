using Godot;
using Solace.addons.solace_core_plugin.lib.sdf_approximation;
using Solace.addons.solace_core_plugin.lib.utilities;

namespace Solace.apps.reference_sdf_approximation;

public partial class SdfApproximateFollower : CharacterBody3D
{
    [Export] private Node3D _directionReference;
    [Export] public Vector3 SkyPoint { get; private set; }

    [Export] public float DirectionalFitWeight { get; set; }
    [Export] public float HitNormalFit { get; set; }
    [Export] public float SurfaceNormalFitWeight { get; set; }
    [Export] public float RaycastDistance { get; set; }
    [Export] public float MovementSpeed { get; set; }
    [Export] public float MovementTurnSpeed { get; set; }
    [Export] public float ObjectRadius { get; set; }

    private Vector3 _currentDirection = Vector3.Zero;

    private float _lastHeight = 0.5f;

    [Export] private bool _drawDebug;
    [Export] private bool _drawDebugSecondary;
    [Export] private bool _immediateOnly;
    public readonly SdfApproximator Approximator = new(100, 0b1);


    public override void _PhysicsProcess(double delta)
    {
        UpdateApproximatorFit();
        var inputLocalSpace = GetLocalSpaceInput();
        UpdateApproximatorSnapshot();
        UpdateCorrectiveMovement(ref inputLocalSpace);


        var upFacingBasis = GetBasisWithDesiredRotation();
        var inputGlobalSpace = upFacingBasis * inputLocalSpace;
        GlobalBasis = upFacingBasis;


        _currentDirection = _currentDirection.Lerp(inputGlobalSpace, (float)(MovementTurnSpeed * delta));

        var velocity = _currentDirection * MovementSpeed;
        Velocity = velocity;

        MoveAndSlide();

        if (_drawDebug)
        {
            DrawDebug();
        }
    }

    private void DrawDebug()
    {
        DebugDraw3D.DrawSphere(GlobalPosition, ObjectRadius, Colors.Gray);
        DebugDraw3D.DrawSphere(GlobalPosition, _lastHeight, Colors.LightYellow);
        DebugDraw3D.DrawSphere(GlobalPosition, RaycastDistance, Colors.White);
    }

    private void UpdateApproximatorFit()
    {
        Approximator.Snapshot.WeightedFitHitNormal = (GlobalBasis * Vector3.Up) * HitNormalFit;
        Approximator.Snapshot.WeightedFitDirection = Velocity * DirectionalFitWeight;
        Approximator.Snapshot.WeightedFitSurfaceNormal = Vector3.Up * SurfaceNormalFitWeight;
        Approximator.Snapshot.ObjectRadius = ObjectRadius;
        Approximator.Snapshot.IgnoreArchived = _immediateOnly;
    }

    private void UpdateCorrectiveMovement(ref Vector3 inputLocalSpace)
    {
        if (Mathf.IsEqualApprox(SurfaceNormalFitWeight, 1f)) return;
        var availableHeight = Approximator.Snapshot.DistanceToSky;
        var currentHeight = Approximator.Snapshot.DistanceToGround;
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
        var lookAction = Input.GetActionStrength("sc_action_secondary");

        var cameraFacingBasis = lookAction == 0
            ? GlobalBasis
            : GlobalBasis.AlignSameLocalVectorGloballyWeighted
            (
                Vector3.Forward,
                _directionReference.GlobalBasis
            );

        var upFacingBasis = cameraFacingBasis.AlignLocalVectorToGlobalVectorWeighted
        (
            Vector3.Up,
            Approximator.Snapshot.SurfaceNormal
        );
        return upFacingBasis;
    }

    private static Vector3 GetLocalSpaceInput()
    {
        var inputDirection = Input.GetVector("sc_move_left", "sc_move_right", "sc_move_backward", "sc_move_forward");
        var verticalDirection = Input.GetAxis("sc_modifier_careful", "sc_modifier_rush");
        var inputLocalSpace = new Vector3(inputDirection.X, verticalDirection, -inputDirection.Y).LimitLength(1f);
        return inputLocalSpace;
    }

    private void UpdateApproximatorSnapshot()
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        Approximator.UpdateSnapshot(spaceState, GlobalPosition, RaycastDistance);
        SkyPoint = Approximator.Snapshot.ProjectedSkyPosition;
    }

    public void ToggleDebug()
    {
        _drawDebug = !_drawDebug;
    }

    public void ToggleImmediateOnly()
    {
        _immediateOnly = !_immediateOnly;
    }
}