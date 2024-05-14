using Godot;
using System;
using Solace.addons.solace_core_plugin.lib.PID;

public partial class PidRigidBody3D : RigidBody3D
{
    [Export] private Vector4 positionGain;

    [Export] private Vector4 rotationGain;

    private Vector4 _pidGain;
    [Export] private Node3D imitationTarget;

    private readonly Vector3PidController _positionPid = new();
    private readonly Vector3PidController _rotationPid = new();


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);


        var targetRotation = imitationTarget.GlobalBasis.GetRotationQuaternion();
        var targetPosition = imitationTarget.GlobalPosition;

        var positionError = targetPosition - GlobalPosition;
        var rotationError = targetRotation * GlobalBasis.GetRotationQuaternion().Inverse();
        _positionPid.Update(positionGain, positionError, LinearVelocity, delta);
        _rotationPid.Update(rotationGain, rotationError.GetEuler(), AngularVelocity, delta);
        ApplyForce(_positionPid.Result);
        ApplyTorque(_rotationPid.Result);
    }
}