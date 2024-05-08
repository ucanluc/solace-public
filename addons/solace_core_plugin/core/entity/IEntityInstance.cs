namespace Solace.addons.solace_core_plugin.core.entity;

/// <summary>
/// A representation of a known entity
/// The instance may provide additional details about the existence of the entity.
/// </summary>
public interface IEntityInstance
{
    public int EntityId { get; set; }
    
}