using Godot;
using System;
using Solace.addons.solace_core_plugin.core;

/// <summary>
/// Does two bone inverse kinematics
/// </summary>
[Tool]
public partial class TwoBoneIK : Node3D
{
    /// <summary>
    /// The first animated bone.
    /// </summary>
    [Export] private Node3D? _rootBone;

    /// <summary>
    /// The second animated bone
    /// </summary>
    [Export] private Node3D? _elbowBone;

    /// <summary>
    /// the point to align with the target, marks the end of the second bone.
    /// </summary>
    [Export] private Node3D? _endEffector;

    /// <summary>
    /// The position to move to in global space.
    /// </summary>
    [Export] private Node3D? _target;

    /// <summary>
    /// The hint position in global space, for choosing an elbow direction. 
    /// </summary>
    [Export] private Node3D? _hint;

    /// <summary>
    /// Force a recheck for initialisation if true.
    /// </summary>
    [Export] private bool _forceRestart = false;

    /// <summary>
    /// No changes are made to the scene if true.
    /// </summary>
    [Export] private bool _pause = false;

    /// <summary>
    /// 
    /// </summary>
    private bool _initialised = false;

    /// <summary>
    /// hint is 'magnetic to the elbow' by default (=false)
    /// when inversed, the elbow avoids the hint. (=true);
    /// </summary>
    [Export] private bool _inverseHint;

    public override void _Ready()
    {
        base._Ready();

        InitialiseIK();
    }

    // ReSharper disable once InconsistentNaming
    private void InitialiseIK()
    {
        if (_rootBone == null || _elbowBone == null || _endEffector == null || _target == null || _hint == null)
        {
            SC.PrintErr(nameof(TwoBoneIK), "One of the target objects is null, aborting.");
            _initialised = false;
            return;
        }

        _initialised = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        CheckRestart();
        CheckRun();
    }

    private void CheckRun()
    {
        if ((!_initialised) || _pause)
        {
            return;
        }

        Solve();
    }

    /// <summary>
    /// Rotates the root and elbow bones to align the end point with the target.
    /// </summary>
    private void Solve()
    {
        var targetVector = _target!.GlobalPosition - _rootBone!.GlobalPosition;
        var targetNormal = targetVector.Normalized();
        var targetLength = targetVector.Length();
        if (targetLength > _elbowBone!.Position.Length() + _endEffector!.Position.Length())
        {
            // cannot reach target; no exact solution exists.
            // we could 'attempt to reach' by putting the bones in a straight line for an approx. solution.
            return;
        }

        var solutionRootAngle =
            LawOfCosines(_elbowBone.Position.Length(), _endEffector.Position.Length(), targetLength);

        var hintVector = _hint!.GlobalPosition - _rootBone.GlobalPosition;
        var hintNormal = hintVector.Normalized();

        // the 'solution triangle' lies on this plane.
        var solutionPlaneNormal = _inverseHint
            ? hintNormal.Cross(targetNormal).Normalized()
            : targetNormal.Cross(hintNormal).Normalized();

        if (solutionPlaneNormal.IsZeroApprox())
        {
            // solution plane cannot be used; so we just assume a plane direction.
            // hint and target might be too close.
            solutionPlaneNormal = Vector3.Up;
        }

        var lookDirection = targetNormal.Rotated(solutionPlaneNormal, solutionRootAngle).Normalized();
        _rootBone.LookAt(_rootBone.GlobalPosition + lookDirection);
        _elbowBone.LookAt(_target.GlobalPosition);
    }

    /// <summary>
    /// Finds one angle from a given triangle, defined by the length of it's edges.
    /// Solves for the angle 'facing' (opposite to) the first edge.
    /// </summary>
    /// <param name="a">Length of first edge</param>
    /// <param name="b">Length of the second edge</param>
    /// <param name="c">Length of the third edge</param>
    /// <returns>The angle in radians. </returns>
    private float LawOfCosines(float a, float b, float c)
    {
        return Mathf.Acos(((b * b) + (c * c) - (a * a)) / (2 * b * c));
    }


    private void CheckRestart()
    {
        if (!_forceRestart) return;

        _forceRestart = false;
        InitialiseIK();
    }
}