using Godot;

namespace Solace.addons.solace_core_plugin.core.entity;

public interface IEntity2D : IEntity
{
    public Vector2 EntityPosition2D { get; set; }
    public float EntityRotationAngle { get; set; }
}