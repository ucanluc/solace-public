using Godot;
using System;
using System.Collections.Generic;
using Solace.addons.solace_core_plugin.core;
using Solace.addons.solace_core_plugin.core.entity;

public partial class CharacterList : Node
{
    private Dictionary<int, CharacterData> _characterDatas = new();

    public override void _EnterTree()
    {
        base._EnterTree();
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        foreach (var characterData in _characterDatas.Values)
        {
            characterData.SyncData(delta);
        }
    }

    public void RegisterEntity3D(int entityId, IEntity3D entity3D)
    {
        CheckCharacter(entityId);
        _characterDatas[entityId].Entity3D = entity3D;
        _characterDatas[entityId].Authority = EntityAuthority.Node3D;
    }

    public void UnregisterEntity3D(int entityId)
    {
        _characterDatas[entityId].Entity3D = null;
        if (_characterDatas[entityId].Entity2D != null)
        {
            _characterDatas[entityId].Authority = EntityAuthority.Node2D;
        }
        else
        {
            _characterDatas[entityId].Authority = EntityAuthority.None;
        }
    }

    private void CheckCharacter(int entityId)
    {
        if (!_characterDatas.ContainsKey(entityId))
        {
            _characterDatas[entityId] = new CharacterData()
            {
                CharacterId = entityId
            };
        }
    }

    public void RegisterEntity2D(int entityId, IEntity2D entity2D)
    {
        CheckCharacter(entityId);

        _characterDatas[entityId].Entity2D = entity2D;
        if (_characterDatas[entityId].Authority == EntityAuthority.None)
        {
            _characterDatas[entityId].Authority = EntityAuthority.Node2D;
        }
    }

    public void UnregisterEntity2D(int entityId)
    {
        _characterDatas[entityId].Entity2D = null;
        _characterDatas[entityId].Authority = EntityAuthority.None;
    }
}

internal class CharacterData
{
    public int CharacterId = -1;
    public EntityAuthority Authority = EntityAuthority.None;
    public IEntity3D? Entity3D;
    public IEntity2D? Entity2D;

    public void SyncData(double delta)
    {
        if (Authority == EntityAuthority.Node3D)
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