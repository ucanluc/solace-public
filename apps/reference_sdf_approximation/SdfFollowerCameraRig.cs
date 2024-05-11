using Godot;

namespace Solace.apps.reference_sdf_approximation;

public partial class SdfFollowerCameraRig : Node3D
{
    [Export] private float _mouseSensitivity = 0.3f;
    private float _cameraXRotation;

    [Export] private Node3D _cameraOrbitNode;
    [Export] private Node3D _cameraNode;
    [Export] private Node3D _followTarget;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            var deltaX = mouseMotion.Relative.Y * _mouseSensitivity;
            var deltaY = -mouseMotion.Relative.X * _mouseSensitivity;

            GlobalRotate(Vector3.Up, Mathf.DegToRad(deltaY));
            if (_cameraXRotation + deltaX > -90 && _cameraXRotation + deltaX < 90)
            {
                _cameraNode.RotateX(Mathf.DegToRad(-deltaX));
                _cameraXRotation += deltaX;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        GlobalPosition = _followTarget.GlobalPosition;
    }
}