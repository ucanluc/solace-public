using Godot;
using Solace.addons.solace_core_plugin.core;

namespace Solace.addons.solace_core_plugin.user_interface.debug;

// ReSharper disable once InconsistentNaming
public partial class DebugUI : Control
{
    [Export] private bool _forceDisableOnStart = true;

    public override void _EnterTree()
    {
        base._EnterTree();
        if (_forceDisableOnStart && Visible)
        {
            SC.PrintWarn(nameof(DebugUI), "Debug UI was visible on creation; disabling it by force.");
            Visible = false;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        // Draw this UI as an overlay (this does NOT change input event handling order.)
        ZIndex = 4096;
    }

    public override void _Input(InputEvent @event)
    {
        // Toggle the visibility of this UI.
        if (@event.IsActionPressed("menu_debug"))
        {
            Visible = !Visible;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}