using Godot;
using System;
using Solace.addons.solace_core_plugin.lib.PID;
using Solace.addons.solace_core_plugin.lib.utilities;

public partial class PidRigidBody3D : RigidBody3D
{
    [Export] public Vector4 positionGain;

    [Export] public  Vector4 rotationGain;

    private Vector4 _pidGain;
    [Export] private Node3D imitationTarget;

    private readonly Vector3PidController _positionPid = new();
    private readonly Vector3PidController _rotationPid = new();
    

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var positionError = imitationTarget.GlobalPosition - GlobalPosition;
        var rotationError = GlobalBasis.FromToEuler(imitationTarget.GlobalBasis);
        _positionPid.Update(positionGain, positionError, LinearVelocity, delta);
        _rotationPid.Update(rotationGain, rotationError, AngularVelocity, delta);
        ApplyForce(_positionPid.Result);
        ApplyTorque(_rotationPid.Result);
    }
}