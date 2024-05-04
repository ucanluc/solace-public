using System;
using Godot;

namespace Solace.addons.solace_core_plugin.core.network;

/// <summary>
/// Global networking functions for the default <see cref="MultiplayerApi"/>
/// Use <see cref="NetworkPeerageNode"/> directly if a secondary connection is required.
/// </summary>
public static class Networking
{
    private static NetworkPeerageNode? _globalPeerage;

    /// <summary>
    /// Attempt to connect to target using the default <see cref="NetworkPeerageNode"/>.
    /// </summary>
    /// <param name="targetAddress">Address to use</param>
    /// <param name="targetPort">Port to use</param>
    public static void ConnectAsClient(string targetAddress, int targetPort)
    {
        if (_globalPeerage == null)
        {
            SC.PrintErr(nameof(Networking), "Cannot connect as client, no global peerage recognised.");
            return;
        }

        _globalPeerage.AttemptConnectionAsClient(targetAddress, targetPort);
    }

    /// <summary>
    /// Attempt to host using the default <see cref="NetworkPeerageNode"/>.
    /// </summary>
    /// <param name="targetPort">Port to use</param>
    public static void ConnectAsHost(int targetPort)
    {
        if (_globalPeerage == null)
        {
            SC.PrintErr(nameof(Networking), "Cannot connect as host, no global peerage recognised.");
            return;
        }

        _globalPeerage.AttemptConnectionAsHost(targetPort);
    }

    /// <summary>
    /// Registers a <see cref="NetworkPeerageNode"/> to handle global networking-related requests by default. 
    /// </summary>
    /// <param name="networkPeerageNode"><see cref="NetworkPeerageNode"/> to assign as default</param>
    public static void RegisterPeerageAsGlobal(NetworkPeerageNode networkPeerageNode)
    {
        if (_globalPeerage != networkPeerageNode)
        {
            SC.PrintWarn(nameof(Networking), "Cannot register peerage; a different peerage is registered globally.");
            return;
        }

        _globalPeerage = networkPeerageNode;
        SC.PrintVerbose(nameof(Networking), "Registered global peerage.");
    }

    /// <summary>
    /// Unregisters a <see cref="NetworkPeerageNode"/> from handling networking status globally.
    /// </summary>
    /// <param name="networkPeerageNode"><see cref="NetworkPeerageNode"/> to remove from being default.</param>
    public static void UnregisterPeerageAsGlobal(NetworkPeerageNode networkPeerageNode)
    {
        if (_globalPeerage == null)
        {
            SC.PrintWarn(nameof(Networking), "Cannot unregister peerage, is already null.");
            return;
        }

        if (_globalPeerage != networkPeerageNode)
        {
            SC.PrintErr(nameof(Networking), "Cannot unregister peerage; a different peerage is registered globally.");
            return;
        }

        _globalPeerage = null;
        SC.PrintVerbose(nameof(Networking), "Unregistered global peerage.");
    }
    
}