namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

/// <summary>
/// Sockets are used to compare different <see cref="WfcOption"/>s for edge/internal fit suitability.
/// Sockets are assumed to face in the same direction on the same axis;
/// A socket on the east side of a tile, and a fitting socket on the west side of another tile are the same.
/// </summary>
public struct WfcSocket
{
    public int SocketId;

    /// <summary>
    /// Check if the sockets are the same.
    /// </summary>
    /// <param name="socket1">first socket to compare</param>
    /// <param name="socket2">second socket to compare</param>
    /// <returns>True if the sockets are the same.</returns>
    public static bool CheckExactFit(WfcSocket socket1, WfcSocket socket2)
    {
        return socket1.SocketId == socket2.SocketId;
    }

    /// <summary>
    /// Checks if the containing socket has equal bits set, or more.
    ///  
    /// True if the containing socket has the same bits set as the included socket.
    /// False if the included socket has set bits that the containing socket does not have set. 
    /// </summary>
    /// <param name="included"></param>
    /// <param name="containing"></param>
    /// <returns></returns>
    public static bool ContainsSameBits(WfcSocket containing, WfcSocket included)
    {
        return ((included.SocketId & containing.SocketId) ^ included.SocketId) > 0;
    }
}