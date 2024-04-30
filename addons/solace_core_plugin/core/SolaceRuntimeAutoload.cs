using Godot;

namespace Solace.addons.solace_core_plugin.core;

public partial class SolaceRuntimeAutoload : Node
{
    public override void _EnterTree()
    {
        SC.Print(nameof(SolaceRuntimeAutoload), "Starting Autoload...");
        SolaceRuntime.Activate();
        base._EnterTree();
    }

    public override void _Ready()
    {
        SC.Print(nameof(SolaceRuntimeAutoload), "Readying Autoload...");
        base._Ready();
    }

    public override void _ExitTree()
    {
        SC.Print(nameof(SolaceRuntimeAutoload), "Exiting Autoload...");
        base._ExitTree();
        SolaceRuntime.Deactivate();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        SolaceRuntime.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        SolaceRuntime.FixedProcess(delta);
    }
}