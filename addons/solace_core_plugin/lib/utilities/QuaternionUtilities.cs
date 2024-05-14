using Godot;

namespace Solace.addons.solace_core_plugin.lib.utilities;

public static class QuaternionUtilities
{
    public static void ToShortWayAround(this ref Quaternion quaternion)
    {
        // Q can be the-long-rotation-around-the-sphere eg. 350 degrees
        // We want the equivalent short rotation eg. -10 degrees
        // Check if rotation is greater than 190 degrees == q.w is negative
        if (!(quaternion.W < 0)) return;
        
        // Convert the quaternion to equivalent "short way around" quaternion
        quaternion.X = -quaternion.X;
        quaternion.Y = -quaternion.Y;
        quaternion.Z = -quaternion.Z;
        quaternion.W = -quaternion.W;
    }
}