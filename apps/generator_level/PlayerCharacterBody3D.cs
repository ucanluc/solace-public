using Godot;
using System;
using Solace.addons.solace_core_plugin.core;
using Solace.addons.solace_core_plugin.core.entity;

public partial class PlayerCharacterBody3D : CharacterBody3D, IEntity3D
{
    [Export] public int EntityId { get; set; } = -1;

    public Vector3 EntityGlobalRotation
    {
        get => Camera.GlobalRotation;
        set => Camera.LookAt(Position + value);
    }

    public Vector3 EntityPosition3D
    {
        get => Position;
        set => Position = value;
    }

    [Export] private CharacterList _characterList;

    [Export] public Node3D Head { get; set; }
    [Export] public Camera3D Camera { get; set; }

    [Export] private float _mouseSensitivity = 0.3f;
    [Export] private float _walkingSpeed = 4.5f;
    [Export] private float _sprintingSpeed = 5.85f;
    [Export] private float _crouchingSpeed = 2.7f;
    // crunch

    [Export] private float _jumpVelocity = 5f;

    [Export] private float _lerpSpeed = 10f;

    private Vector3 _currentDirection = Vector3.Zero;

    private float _currentSpeed;

    private float _cameraXRotation;
    private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


    public override void _EnterTree()
    {
        base._EnterTree();
        _characterList.RegisterEntity3D(EntityId, this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _characterList.UnregisterEntity3D(EntityId);
    }

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            var mouseMotion = @event as InputEventMouseMotion;
            var deltaX = mouseMotion.Relative.Y * _mouseSensitivity;
            var deltaY = -mouseMotion.Relative.X * _mouseSensitivity;

            Head.RotateY(Mathf.DegToRad(deltaY));
            if (_cameraXRotation + deltaX > -90 && _cameraXRotation + deltaX < 90)
            {
                Camera.RotateX(Mathf.DegToRad(-deltaX));
                _cameraXRotation += deltaX;
            }
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;
        if (!IsOnFloor())
        {
            velocity.Y -= _gravity * (float)delta;
        }

        if (Input.IsActionPressed("sc_modifier_careful"))
        {
            _currentSpeed = _crouchingSpeed;
        }
        else if (Input.IsActionPressed("sc_modifier_rush"))
        {
            _currentSpeed = _sprintingSpeed;
        }
        else
        {
            _currentSpeed = _walkingSpeed;
        }

        if (Input.IsActionJustPressed("sc_action_jump") && IsOnFloor())
        {
            velocity.Y = _jumpVelocity;
        }

        var inputDirection = Input.GetVector("sc_move_left", "sc_move_right", "sc_move_backward", "sc_move_forward");
        var desiredDirection = Head.Transform.Basis * new Vector3(inputDirection.X, 0, -inputDirection.Y).Normalized();
        _currentDirection = _currentDirection.Lerp(desiredDirection, (float)(_lerpSpeed * delta));


        // var direction = Vector3.Zero;
        // direction += inputDirection.X * Head.GlobalBasis.X;
        //
        // //forward is negative Z
        // direction += inputDirection.Y * -Head.GlobalBasis.Z;

        velocity.X = _currentDirection.X * _currentSpeed;
        velocity.Z = _currentDirection.Z * _currentSpeed;

        Velocity = velocity;
        MoveAndSlide();
    }
}