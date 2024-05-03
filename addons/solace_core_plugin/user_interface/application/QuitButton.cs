using Godot;
using Solace.addons.solace_core_plugin.core;

namespace Solace.addons.solace_core_plugin.user_interface.application;

public partial class QuitButton : Button
{
    [Export] private bool _canForceQuit = true;

    public override void _Ready()
    {
        Pressed += OnQuitGameButtonPressed;
    }

    private void OnQuitGameButtonPressed()
    {
        // request a graceful quit, force quit otherwise.
        if (!SC.QuitApplication() && _canForceQuit)
        {
            // force quit via godot.
            GetTree().Quit();
        }
    }
}