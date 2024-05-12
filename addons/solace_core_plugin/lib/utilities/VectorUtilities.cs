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
    /// <returns>The distance from the point to the plane; parallel to the plane normal.</returns>
    public static float DistanceToPlane(this Vector3 point, Vector3 planeOrigin, Vector3 planeNormal)
    {
        var relativePoint = point - planeOrigin;
        var planeParallel = relativePoint.Project(planeNormal).Normalized();
        var pointHeightVector = relativePoint.Project(planeParallel);
        var pointHeightSign = pointHeightVector.Normalized().Dot(planeNormal) > 0 ? 1 : -1;
        return pointHeightSign * pointHeightVector.Length();
    }

    /// <summary>
    /// Get the angle and axis of the rotation that would align the two given vectors.
    /// Angle returned is positive counter clockwise, negative clockwise; right hand rule.
    /// </summary>
    /// <param name="currentVector">Vector to rotate from</param>
    /// <param name="targetVector">Vector to rotate to</param>
    /// <param name="angleDiff">Signed difference in angle, in radians</param>
    /// <param name="rotationAxis">Normalised axis vector for the rotation</param>
    /// <returns></returns>
    public static void ToAngleAxisDifference(Vector3 currentVector,
        Vector3 targetVector, out float angleDiff, out Vector3 rotationAxis)
    {
        rotationAxis = currentVector.Cross(targetVector).Normalized();
        if (rotationAxis.IsZeroApprox())
        {
            // we do not know if the vectors are completely opposite or completely aligned;
            // so we use an arbitrary vector.
            rotationAxis = Vector3.Up;
        }

        angleDiff = currentVector.SignedAngleTo(targetVector, rotationAxis);
    }

    /// <summary>
    /// Decomposes given position relative to a plane;
    /// giving height to plane surface, orthogonal vector, and plane parallel vector.
    /// The height may be negative, depending on alignment with the plane's normal.
    /// </summary>
    /// <param name="pointToProject">Point coordinates to project on to the plane</param>
    /// <param name="referencePointOnPlane">A point on the plane to get parallel alignment to.</param>
    /// <param name="planeNormal">The normal of the plane surface.</param>
    /// <param name="pointHeightVector">Height vector of the first point, relative to the plane surface</param>
    /// <param name="planeParallelTranslation">
    /// Translation of the projected point, from the reference point.
    /// </param>
    /// <returns>The distance from the point to the plane; parallel with the plane normal.</returns>
    public static float DecomposeWithPlane(
        this Vector3 pointToProject,
        Vector3 referencePointOnPlane, Vector3 planeNormal,
        out Vector3 pointHeightVector,
        out Vector3 planeParallelTranslation
    )
    {
        // nearly the same as getting the point's height to plane, just with extra outputs.
        var relativePoint = pointToProject - referencePointOnPlane;
        planeParallelTranslation = relativePoint.Project(planeNormal);
        pointHeightVector = relativePoint.Project(planeParallelTranslation.Normalized());
        var pointHeightSign = pointHeightVector.Normalized().Dot(planeNormal) > 0 ? 1 : -1;
        var distanceToPlane = pointHeightSign * pointHeightVector.Length();

        return distanceToPlane;
    }
}