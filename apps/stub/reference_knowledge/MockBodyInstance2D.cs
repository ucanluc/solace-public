using Godot;
using Solace.addons.solace_core_plugin.core.entity;

namespace Solace.apps.stub.reference_knowledge;

public partial class MockBodyInstance2D : AnimatableBody2D, IEntityInstance2D
{
    [Export] private Node2D rotationTarget;
    [Export] public int EntityId { get; set; } = -1;

    [Export] private float targetRotation;

    public Vector2 EntityPosition2D
    {
        get => Position;
        set => Position = value;
    }

    public float EntityRotationAngle
    {
        get => rotationTarget.GlobalRotation;
        set => targetRotation = value;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        rotationTarget.Rotate(targetRotation - rotationTarget.GlobalRotation);
    }
}