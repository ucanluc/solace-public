using Godot;

namespace Solace.addons.solace_core_plugin.core.application;

/// <summary>
/// Defines TODO:'application'
/// Instances of this resource defines an application.
/// </summary>
[Tool] // make it capable of running in the editor
[GlobalClass] // makes it easier to use the resource in the inspector
public partial class AppDefinitionResource : Resource
{
    [Export] private PackedScene? _appEntryNode;

    /// <summary>
    /// Mandatory constructor.
    /// An empty parameterless constructor is needed for global resources to be recognised by Godot.
    /// </summary>
    public AppDefinitionResource()
    {
    }

    /// <summary>
    /// Loads and sets up the defined app
    /// </summary>
    /// <param name="parent">Parent node to attach the app's main node</param>
    public void LoadApp(Node parent)
    {
        if (_appEntryNode == null)
        {
            SC.Print(nameof(AppDefinitionResource),
                $"Cannot load app; provide a scene to load in {ResourceName}.");
            return;
        }

        var nodeInstance = _appEntryNode.Instantiate();
        
        parent.AddChild(nodeInstance);
    }
    
}