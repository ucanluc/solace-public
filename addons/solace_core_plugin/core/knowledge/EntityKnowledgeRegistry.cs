using System;
using System.Collections.Generic;
using Solace.addons.solace_core_plugin.core.entity;

namespace Solace.addons.solace_core_plugin.core.knowledge;

public class EntityKnowledgeRegistry
{
    private Dictionary<int, EntityData> _entityDatas = new();

    private void RegisterEntity3D(IEntityInstance3D entityInstance3D)
    {
        UpdateEntityExistence(entityInstance3D.EntityId);
        _entityDatas[entityInstance3D.EntityId].Entity3D = entityInstance3D;
        _entityDatas[entityInstance3D.EntityId].SyncAuthority = EntitySyncAuthority.Node3D;
    }

    public void UnregisterEntity3D(int entityId)
    {
        _entityDatas[entityId].Entity3D = null;
        if (_entityDatas[entityId].Entity2D != null)
        {
            _entityDatas[entityId].SyncAuthority = EntitySyncAuthority.Node2D;
        }
        else
        {
            _entityDatas[entityId].SyncAuthority = EntitySyncAuthority.None;
        }
    }

    private void UpdateEntityExistence(int entityId)
    {
        if (!_entityDatas.ContainsKey(entityId))
        {
            _entityDatas[entityId] = new EntityData()
            {
                EntityId = entityId
            };
        }
    }

    private void RegisterEntity2D(IEntityInstance2D entityInstance2D)
    {
        UpdateEntityExistence(entityInstance2D.EntityId);

        _entityDatas[entityInstance2D.EntityId].Entity2D = entityInstance2D;
        if (_entityDatas[entityInstance2D.EntityId].SyncAuthority == EntitySyncAuthority.None)
        {
            _entityDatas[entityInstance2D.EntityId].SyncAuthority = EntitySyncAuthority.Node2D;
        }
    }

    public void UnregisterEntity2D(int entityId)
    {
        _entityDatas[entityId].Entity2D = null;
        _entityDatas[entityId].SyncAuthority = EntitySyncAuthority.None;
    }

    public void Update()
    {
        foreach (var characterData in _entityDatas.Values)
        {
            characterData.SyncData();
        }
    }

    public void Register(IEntityInstance entity)
    {
        if (entity is IEntityInstance3D entityInstance3D)
        {
            RegisterEntity3D(entityInstance3D);
        }

        if (entity is IEntityInstance2D entityInstance2D)
        {
            RegisterEntity2D(entityInstance2D);
        }
    }
}