using Godot;

namespace Solace.addons.solace_core_plugin.core.entity;

/// <summary>
/// A representation of a known entity in 3d space.
/// </summary>
public interface IEntityInstance3D : IEntityInstance
{
    public Vector3 EntityPosition3D { get; set; }
    public Vector3 EntityGlobalRotation { get; set; }
}