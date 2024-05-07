using Godot;
using System;
using Solace.addons.solace_core_plugin.core.entity;

public partial class MockBody2D : AnimatableBody2D, IEntity2D
{
    [Export] private CharacterList _characterList;
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

    public override void _EnterTree()
    {
        base._EnterTree();
        _characterList.RegisterEntity2D(EntityId, this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _characterList.UnregisterEntity2D(EntityId);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        rotationTarget.Rotate(targetRotation - rotationTarget.GlobalRotation);
    }
}