namespace Solace.addons.solace_core_plugin.lib.generator;

public struct TilingSocket
{
    public int SocketId;

    public static bool CheckSocketsFit(TilingSocket socket1, TilingSocket socket2)
    {
        return socket1.SocketId == socket2.SocketId;
    }
}