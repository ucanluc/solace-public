using Godot;

namespace Solace.addons.solace_core_plugin.core.entity;

/// <summary>
/// A representation of a known entity in 2d space.
/// </summary>
public interface IEntityInstance2D : IEntityInstance
{
    public Vector2 EntityPosition2D { get; set; }
    public float EntityRotationAngle { get; set; }
    
}