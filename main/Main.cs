using Godot;
using Solace.addons.solace_core_plugin.core;
using Solace.addons.solace_core_plugin.core.application;

namespace Solace.main;

public partial class Main : Node
{
    [Export] private AppDefinitionResource? _appToLoad;

    public override void _Ready()
    {
        base._Ready();
        if (_appToLoad == null)
        {
            SC.Print(nameof(Main), "Cannot load; No app provided");
            return;
        }

        _appToLoad.LoadApp(this);
    }
}