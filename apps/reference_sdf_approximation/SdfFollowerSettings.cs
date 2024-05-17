using Godot;
using System;
using Solace.addons.solace_core_plugin.lib.sdf_approximation;
using Solace.apps.reference_sdf_approximation;
using Solace.content.user_interface.validation;

public partial class SdfFollowerSettings : Node
{
    [Export] private SdfApproximateFollower follower;

    [Export] private NumeralHSlider _velocitySlider;
    [Export] private NumeralHSlider _localUpSlider;
    [Export] private NumeralHSlider _globalSlider;
    [Export] private NumeralHSlider _radiusSlider;
    [Export] private NumeralHSlider _rayDistanceSlider;
    [Export] private NumeralHSlider _rayCountSlider;
    [Export] private NumeralHSlider _moveSpeedSlider;
    [Export] private Button _toggleRayDebug;
    [Export] private Button _toggleCharacterDebug;
    [Export] private Button _toggleImmediateOnly;


    public override void _Ready()
    {
        base._Ready();
        _velocitySlider.ValueChanged += value => follower.DirectionalFitWeight = (float)(value / 100);
        _localUpSlider.ValueChanged += value => follower.HitNormalFit = (float)(value / 100);
        _globalSlider.ValueChanged += value => follower.SurfaceNormalFitWeight = (float)(value / 100);
        _radiusSlider.ValueChanged += value => follower.ObjectRadius = (float)value / 100;
        _rayDistanceSlider.ValueChanged += value => follower.RaycastDistance = (float)value;
        _rayCountSlider.ValueChanged += value => follower.Approximator.RecreateTrackers((int)value, 0b1);
        _moveSpeedSlider.ValueChanged += value => follower.MovementSpeed = (float)value;
        _toggleRayDebug.Pressed += () => follower.Approximator.Snapshot.ToggleDebug();
        _toggleCharacterDebug.Pressed += () => follower.ToggleDebug();
        _toggleImmediateOnly.Pressed += () => follower.ToggleImmediateOnly();
        follower.Approximator.Snapshot.DrawDebugLines = true;
        follower.Approximator.Snapshot.IgnoreArchived = true;
    }
}