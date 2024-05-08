using Godot;

namespace Solace.addons.solace_core_plugin.core.entity;

/// <summary>
/// An entity that is known to exist,
/// which can have additional data, and be synced across representations (instances)
/// </summary>
internal class EntityData
{
    public int EntityId = -1;
    public EntitySyncAuthority SyncAuthority = EntitySyncAuthority.None;
    public IEntityInstance3D? Entity3D;
    public IEntityInstance2D? Entity2D;

    public void SyncData()
    {
        if (SyncAuthority == EntitySyncAuthority.Node3D)
        {
            if (Entity2D != null && Entity3D != null)
            {
                var rotation3D = Entity3D.EntityGlobalRotation.Y;
                var xPosition = Entity3D.EntityPosition3D.X;
                var zPosition = Entity3D.EntityPosition3D.Z;
                Entity2D.EntityRotationAngle = ConvertYRotation3DTo2D(rotation3D);
                Entity2D.EntityPosition2D = ConvertPosition3DTo2D(xPosition, zPosition);
            }
        }
    }

    private static Vector2 ConvertPosition3DTo2D(float xPosition, float zPosition)
    {
        return new Vector2(
            xPosition * SolaceConstants.GodotPixelsPerMeter,
            zPosition * SolaceConstants.GodotPixelsPerMeter
        );
    }

    private static float ConvertYRotation3DTo2D(float f)
    {
        return -f - Mathf.DegToRad(90);
    }
}