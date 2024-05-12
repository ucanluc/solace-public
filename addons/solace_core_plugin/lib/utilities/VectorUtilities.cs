using Godot;

namespace Solace.addons.solace_core_plugin.lib.utilities;

public static class VectorUtilities
{
    /// <summary>
    /// Creates a dot product normalised between 0~1.
    /// The default -1~1 range for normalised vectors is mapped to 0~1.
    /// </summary>
    /// <param name="targetVector">First vector</param>
    /// <param name="fitVector">Second vector</param>
    /// <returns>The dot product normalised to 0~1</returns>
    public static float Dot01(Vector3 targetVector, Vector3 fitVector)
    {
        return ((targetVector.Dot(fitVector) + 1f) / 2f).Clamp01();
    }

    /// <summary>
    /// Give the distance to the projection of the point on the plane.
    /// </summary>
    /// <param name="point">Point coordinates</param>
    /// <param name="planeOrigin">A point on the plane</param>
    /// <param name="planeNormal">The normal of the plane surface.</param>
    /// <returns></returns>
    public static float DistanceToPlane(this Vector3 point, Vector3 planeOrigin, Vector3 planeNormal)
    {
        var relativePoint = point - planeOrigin;
        var planeParallel = relativePoint.Project(planeNormal).Normalized();
        var pointHeightVector = relativePoint.Project(planeParallel);
        var pointHeightSign = pointHeightVector.Normalized().Dot(planeNormal) > 0 ? 1 : -1;
        return pointHeightSign * pointHeightVector.Length();
    }
}