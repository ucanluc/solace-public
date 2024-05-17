using Godot;
using System;
using Solace.content.user_interface.validation;

public partial class PidBodySettings : Node
{
    [Export] private NumeralHSlider _massSlider;
    [Export] private NumeralHSlider _posPSlider;
    [Export] private NumeralHSlider _posISlider;
    [Export] private NumeralHSlider _posDSlider;
    [Export] private NumeralHSlider _posMaxSlider;
    [Export] private NumeralHSlider _rotPSlider;
    [Export] private NumeralHSlider _rotISlider;
    [Export] private NumeralHSlider _rotDSlider;
    [Export] private NumeralHSlider _rotMaxSlider;

    [Export] private PidRigidBody3D _pidRigidbody;

    public override void _Ready()
    {
        base._Ready();
        _massSlider.ValueChanged += value =>
            _pidRigidbody.Mass = (float)value;

        _posPSlider.ValueChanged += value =>
            _pidRigidbody.positionGain = _pidRigidbody.positionGain with { X = (float)value };
        _posISlider.ValueChanged += value =>
            _pidRigidbody.positionGain = _pidRigidbody.positionGain with { Y = (float)value };
        _posDSlider.ValueChanged += value =>
            _pidRigidbody.positionGain = _pidRigidbody.positionGain with { Z = (float)value };
        _posMaxSlider.ValueChanged += value =>
            _pidRigidbody.positionGain = _pidRigidbody.positionGain with { W = (float)value };


        _rotPSlider.ValueChanged += value =>
            _pidRigidbody.positionGain = _pidRigidbody.rotationGain with { X = (float)value };
        _rotISlider.ValueChanged += value =>
            _pidRigidbody.positionGain = _pidRigidbody.rotationGain with { Y = (float)value };
        _rotDSlider.ValueChanged += value =>
            _pidRigidbody.positionGain = _pidRigidbody.rotationGain with { Z = (float)value };
        _rotMaxSlider.ValueChanged += value =>
            _pidRigidbody.positionGain = _pidRigidbody.rotationGain with { W = (float)value };
    }
}