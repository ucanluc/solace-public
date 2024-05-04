using Godot;

namespace Solace.addons.solace_core_plugin.core.network;

/// <summary>
/// Instances given scene for every connected peer (including this node)
/// Each instance of this node is a potential Client/Host
/// </summary>
public partial class NetworkPeerageNode : Node
{
    // TODO:
    /// <summary>
    /// Locally registered peerage will not be accessible globally.
    /// This is suitable if multiple concurrent connections are desired.
    /// </summary>
    [Export] private bool _registerLocally = false;

    // TODO: register server-only functionality separately for headless server support
    private readonly ENetMultiplayerPeer _eNetPeer = new ENetMultiplayerPeer();

    // TODO: register connected clients for arbitrary node spawns 
    [Export] private PackedScene? _networkPeerNodeScene;

    public override void _EnterTree()
    {
        base._EnterTree();
        if (!_registerLocally)
        {
            Networking.RegisterPeerageAsGlobal(this);
        }
        else
        {
            SC.PrintWarn(nameof(NetworkPeerageNode), "Network peerage assigned to work locally");
        }
    }

    public override void _Ready()
    {
        base._Ready();

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        // client only
        Multiplayer.ConnectedToServer += OnConnectedAsClient;
        Multiplayer.ConnectionFailed += OnConnectionFailedAsClient;
        Multiplayer.ServerDisconnected += OnDisconnectedAsClient;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (!_registerLocally)
        {
            Networking.UnregisterPeerageAsGlobal(this);
        }
        else
        {
            SC.PrintWarn(nameof(NetworkPeerageNode), "Locally assigned network peerage removed from tree.");
        }
    }


    #region Multiplayer Event Handlers

    private void OnPeerConnected(long peerId)
    {
        SC.Print(nameof(NetworkPeerageNode), $"Peer connected: {peerId}");
        // The signals give peerId as a long, but the rest of the Multiplayer API uses int; so we have to cast it.
        AddPeer((int)peerId);
    }

    private void OnPeerDisconnected(long peerId)
    {
        SC.Print(nameof(NetworkPeerageNode), $"Peer disconnected: {peerId}");
        // The signals give peerId as a long, but the rest of the Multiplayer API uses int; so we have to cast it.
        RemovePeer((int)peerId);
    }


    private void OnDisconnectedAsClient()
    {
        SC.Print(nameof(NetworkPeerageNode), "Disconnected from server.");
        RemoveAllPeers();
    }


    private void OnConnectionFailedAsClient()
    {
        SC.Print(nameof(NetworkPeerageNode), "Connection to server failed.");
        RemoveAllPeers();
    }

    private void OnConnectedAsClient()
    {
        SC.Print(nameof(NetworkPeerageNode), "Connected to server.");
    }

    #endregion

    #region Network Event Triggers

    internal Error AttemptConnectionAsClient(string targetAddress, int targetPort)
    {
        var result = _eNetPeer.CreateClient(targetAddress, targetPort);
        if (result != Error.Ok) return result;

        CreateOwnPeer();

        return result;
    }


    internal Error AttemptConnectionAsHost(int targetPort)
    {
        var result = _eNetPeer.CreateServer(targetPort);
        Multiplayer.MultiplayerPeer = _eNetPeer;

        if (result != Error.Ok) return result;

        CreateOwnPeer();

        return result;
    }

    private void UpnpSetup(int activePort)
    {
        // TODO: UPNP on it's own thread
        var upnp = new Upnp();
        var discoverResult = upnp.Discover();

        if (discoverResult != (int)Upnp.UpnpResult.Success)
        {
            GD.PrintErr($"Upnp discover failed! Error: {discoverResult}");
            return;
        }


        if (!(upnp.GetGateway() != null && upnp.GetGateway().IsValidGateway()))
        {
            GD.PrintErr("Upnp invalid gateway");
            return;
        }

        var mapResult = upnp.AddPortMapping(activePort);

        if (mapResult != (int)Upnp.UpnpResult.Success)
        {
            GD.PrintErr($"Upnp port mapping failed! Error: {mapResult}");
            return;
        }

        GD.Print($"Success! Join address: {upnp.QueryExternalAddress()} ");
    }

    #endregion

    #region Peer Handling

    private void CreateOwnPeer()
    {
        Multiplayer.MultiplayerPeer = _eNetPeer;
        var uid = Multiplayer.GetUniqueId();
        SC.Print(nameof(NetworkPeerageNode), $"Creating owned peer as {uid}.");
        AddPeer(uid);
    }

    private void AddPeer(int peerId)
    {
        if (_networkPeerNodeScene == null)
        {
            SC.PrintErr(nameof(NetworkPeerageNode), $"Cannot add a peer for id {peerId}, no scene is provided.");
            return;
        }

        var peerNode = _networkPeerNodeScene.Instantiate();


        // Preserving peerId within the peer name allows supporting other spawning methods such as MultiplayerSpawner.
        peerNode.Name = peerId.ToString();
        peerNode.SetMultiplayerAuthority(peerId);

        AddChild(peerNode);
        RegisterPeer(peerNode);
    }

    private void RegisterPeer(Node playerInstance)
    {
        SC.Print(nameof(NetworkPeerageNode), $"Registered peer: {playerInstance.GetMultiplayerAuthority()}");
    }

    private void RemovePeer(long peerId)
    {
        SC.Print(nameof(NetworkPeerageNode), $"Removing peer: {peerId}");
        var peer = GetNodeOrNull<Node>(peerId.ToString());
        peer?.QueueFree();
    }


    private void RemovePeer(Node peer)
    {
        SC.Print(nameof(NetworkPeerageNode), $"Removing peer: {peer.GetMultiplayerAuthority()}");
        peer.QueueFree();
    }

    private void RemoveAllPeers()
    {
        var children = GetChildren();
        SC.Print(nameof(NetworkPeerageNode), $"Removing all {children.Count} peers.");

        foreach (var child in children)
        {
            if (child is null) continue;
            RemovePeer(child);
        }
    }

    #endregion
}