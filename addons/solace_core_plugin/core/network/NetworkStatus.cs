using Godot;

namespace Solace.addons.solace_core_plugin.core.network;

public partial class NetworkStatus : RefCounted
{
    [Signal] //signals defined in C# need to end with 'EventHandler' for Godot to register them.
    public delegate void OnNetworkStatusChangedEventHandler(NetworkStatus status);

    public string ConnectionText { get; private set; } = "Uninitialised";

    private void AnnounceStatusChange()
    {
        EmitSignal(SignalName.OnNetworkStatusChanged, this);
    }
}