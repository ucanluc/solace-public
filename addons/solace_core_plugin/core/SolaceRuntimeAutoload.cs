using Godot;

namespace Solace.addons.solace_core_plugin.core;

/// <summary>
/// Attaches the Solace Runtime to Godot's lifecycle.
/// </summary>
/// <remarks>Assumed to be a Autoload in Godot's settings.</remarks>
public partial class SolaceRuntimeAutoload : Node
{
    public override void _EnterTree()
    {
        SC.Print(nameof(SolaceRuntimeAutoload), "Starting Autoload...");
        base._EnterTree();
        SolaceRuntime.Activate();
    }

    public override void _Ready()
    {
        SC.Print(nameof(SolaceRuntimeAutoload), "Readying Autoload...");
        base._Ready();
    }

    public override void _ExitTree()
    {
        SC.Print(nameof(SolaceRuntimeAutoload), "Exiting Autoload...");
        SolaceRuntime.Deactivate();
        base._ExitTree();
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