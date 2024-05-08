using Godot;
using System;
using Solace.addons.solace_core_plugin.core;
using Solace.addons.solace_core_plugin.core.entity;
using Solace.addons.solace_core_plugin.core.knowledge;

public partial class SpatialKnowledgeScene : Node
{
    private EntityKnowledgeRegistry _entityKnowledgeRegistry=new EntityKnowledgeRegistry();

    public override void _Ready()
    {
        base._Ready();

        var children = GetChildren();
        foreach (var child in children)
        {
            if (child is IEntityInstance entity)
            {
                _entityKnowledgeRegistry.Register(entity);
            }
        }
    }


    public override void _EnterTree()
    {
        base._EnterTree();
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _entityKnowledgeRegistry.Update();
    }
}