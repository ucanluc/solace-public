using Godot;

namespace Solace.addons.solace_core_plugin.lib.utilities;

public static class TrigUtilities
{
    /// <summary>
    /// Finds one angle from a given triangle, defined by the length of it's edges.
    /// Solves for the angle 'facing' (opposite to) the first edge.
    /// </summary>
    /// <param name="a">Length of first edge</param>
    /// <param name="b">Length of the second edge</param>
    /// <param name="c">Length of the third edge</param>
    /// <returns>The angle in radians. </returns>
    public static float LawOfCosines(float a, float b, float c)
    {
        return Mathf.Acos(((b * b) + (c * c) - (a * a)) / (2 * b * c));
    }
}