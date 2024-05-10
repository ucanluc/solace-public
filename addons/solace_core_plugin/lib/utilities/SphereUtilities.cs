using System.Collections.Generic;
using Godot;

namespace Solace.addons.solace_core_plugin.lib.utilities;

public static class SphereUtilities
{
    /// <summary>
    /// Create a list of n points (approximately) evenly distributed on the surface of the unit sphere.
    /// I.e. lengths of the points are normalised. 
    /// Adapted from https://stackoverflow.com/a/76572678
    /// </summary>
    /// <param name="pointsCount">Number of points to generate</param>
    /// <param name="offset">Starting offset; 0.5 recommended.</param>
    /// <returns>Evenly distributed points on the unit sphere's surface.</returns>
    public static Vector3[] GetPointsOnUnitSphere(int pointsCount, float offset = 0.5f)
    {
        var points = new List<Vector3>();

        var thetaIncrement = Mathf.Pi * (1f + Mathf.Sqrt(5f));
        for (var i = 0; i < pointsCount; i++)
        {
            var index = i + offset;
            var phi = Mathf.Acos(1f - 2f * index / pointsCount);

            var theta = thetaIncrement * index;
            var x = Mathf.Cos(theta) * Mathf.Sin(phi);
            var y = Mathf.Sin(theta) * Mathf.Sin(phi);
            var z = Mathf.Cos(phi);
            points.Add(new Vector3(x, y, z).Normalized());
        }

        return points.ToArray();
    }
}