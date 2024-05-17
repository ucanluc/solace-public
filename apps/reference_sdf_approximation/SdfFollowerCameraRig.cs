using System;
using Godot;
using Solace.addons.solace_core_plugin.lib.utilities;

namespace Solace.apps.reference_sdf_approximation;

public partial class SdfFollowerCameraRig : Node3D
{
    [Export] private float _mouseSensitivity = 0.3f;
    private float _cameraXRotation;

    [Export] private Node3D _cameraOrbitNode;
    [Export] private Node3D _cameraNode;
    [Export] private SdfApproximateFollower _followTarget;
    [Export] private Control _hud;

    public override void _Ready()
    {
        HideHud();

        _hud.GuiInput += HudOnGuiInput;
        return;

        void HudOnGuiInput(InputEvent @event)
        {
            if (@event is not InputEventMouseButton { Pressed: true }) return;
            HideHud();
        }
    }

    private void HideHud()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;

        SetProcessInput(true);
    }

    private void ShowHud()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        SetProcessInput(false);
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

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Input.IsActionJustPressed("sc_menu_quit")) return;

        if (!_hud.Visible)
        {
            ShowHud();
        }
        else
        {
            HideHud();
        }

        _hud.Visible = !_hud.Visible;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        ProjectCameraOrbit();
    }

    private void ProjectCameraOrbit()
    {
        GlobalPosition = _followTarget.GlobalPosition;


        _cameraOrbitNode.GlobalPosition =
            _cameraOrbitNode.GlobalPosition.ProjectPositionToSphereSurface(GlobalPosition,
                _followTarget.RaycastDistance - 1f);
        _cameraOrbitNode.Position = _cameraOrbitNode.Position with
        {
            Y = _followTarget.SkyPoint.DistanceToPlane(GlobalPosition, Vector3.Up) / 2
        };
        var space = GetWorld3D().DirectSpaceState;
        var result =
            space.IntersectRay(
                PhysicsRayQueryParameters3D.Create(GlobalPosition, _cameraOrbitNode.GlobalPosition, 0b1));

        var hasHit = result.Count > 0;
        if (hasHit)
        {
            var hitPosition = (Vector3)result["position"];
            _cameraOrbitNode.GlobalPosition = hitPosition;
        }
    }
}