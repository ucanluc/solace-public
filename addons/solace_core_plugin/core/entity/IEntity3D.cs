using Godot;

namespace Solace.addons.solace_core_plugin.core.entity;

public interface IEntity3D : IEntity
{
    public Vector3 EntityPosition3D { get; set; }
    public Vector3 EntityGlobalRotation { get; set; }
}