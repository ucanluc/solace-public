using System;
using Godot;

namespace Solace.addons.solace_core_plugin.lib.sdf_approximation;

public class SdfSnapshot
{
    /// <summary>
    /// Optional; tweaks weights based on distance to origin.
    /// </summary>
    public float ObjectRadius { private get; set; }

    /// <summary>
    /// Optional; set to prioritise hits with aligned normals.
    /// If length = 0; Disabled
    /// If length is 0~1; Mild adjustment
    /// If length is 1; Weights are between 0~1 depending on alignment. 
    /// If length above 1; Starts ignoring misaligned vectors completely. 
    /// </summary>
    public Vector3 NormalFitVector { private get; set; }

    /// <summary>
    /// Optional; set to prioritise hits in the aligned direction.
    /// If length = 0; Disabled
    /// If length is 0~1; Mild adjustment
    /// If length is 1; Weights are between 0~1 depending on alignment. 
    /// If length above 1; Starts ignoring misaligned vectors completely. 
    /// </summary>
    public Vector3 DirFitVector { private get; set; }

    /// <summary>
    /// Derived output:
    /// This direction is "UP"; If you want to stay parallel to the surface.
    /// According to the surface normals nearby.
    /// </summary>
    public Vector3 GroundNormal { get; private set; }

    /// <summary>
    /// Derived output:
    /// This direction is "UP"; If you want to get away from any solid objects.
    /// According to the most empty space available.
    /// This is the main output of a Signed Distance field; if we were using one.
    /// </summary>
    public Vector3 SkyDirection { get; private set; }

    /// <summary>
    /// Derived output;
    /// This point is the assumed center of the "ground plane" in global space
    /// According to the raycast hit locations.
    /// </summary>
    public Vector3 GroundPoint { get; private set; }

    /// <summary>
    /// Derived output;
    /// This is our height from the assumed ground plane.
    /// May be negative, depending on assumed ground, and object radius.
    /// </summary>
    public float GroundHeight { get; private set; }

    private float _groundPointWeightTotal, _raycastDist;
    private Vector3 _origin;

    /// <summary>
    /// Derives weighted values from the given tracker's status.
    /// 
    /// </summary>
    /// <param name="tracker"></param>
    public void IntegrateTracker(SdfRaycastTracker tracker)
    {
        if (!tracker.HasCurrentHit)
        {
            SkyDirection += tracker.RaycastDirection;
            return;
        }

        var hitVector = (tracker.HitPosition - _origin);
        var maxDistance = Mathf.Clamp(_raycastDist - ObjectRadius, 0, _raycastDist);
        var hitDistance = Mathf.Clamp(hitVector.Length() - ObjectRadius, 0, maxDistance);
        var hitNormal = tracker.HitNormal.Normalized();
        var hitDirection = hitVector.Normalized();

        var groundWeight =
            hitDistance > float.Epsilon
                ? Clamp01(maxDistance / hitDistance)
                : 1f;

        var skyWeight =
            maxDistance > float.Epsilon
                ? Clamp01(hitDistance / maxDistance)
                : 0f;

        var normalFitWeight =
            NormalFitVector.Length() > float.Epsilon
                ? Dot01(hitNormal, NormalFitVector)
                : 1;
        var dirFitWeight =
            DirFitVector.Length() > float.Epsilon
                ? Dot01(hitDirection, DirFitVector)
                : 1;

        // weights are exponential akin to "Least squares fitting";
        // https://mathworld.wolfram.com/LeastSquaresFitting.html
        groundWeight *= groundWeight;
        skyWeight *= skyWeight;
        normalFitWeight *= normalFitWeight;
        dirFitWeight *= dirFitWeight;

        var trackerWeight = groundWeight * dirFitWeight * normalFitWeight;
        GroundNormal += hitNormal * trackerWeight;
        GroundPoint += hitVector;
        _groundPointWeightTotal += trackerWeight;

        SkyDirection += (-hitDirection) * skyWeight;
    }

    /// <summary>
    /// Creates a dot product normalised between 0~1.
    /// The default -1~1 range for normalised vectors is mapped to 0~1.
    /// </summary>
    /// <param name="targetVector">First vector</param>
    /// <param name="fitVector">Second vector</param>
    /// <returns>The dot product normalised to 0~1</returns>
    private static float Dot01(Vector3 targetVector, Vector3 fitVector)
    {
        return Clamp01((targetVector.Dot(fitVector) + 1f) / 2f);
    }


    private static float Clamp01(float value)
    {
        return Mathf.Clamp(value, 0, 1);
    }


    public void Clear(Vector3 queryOrigin, float raycastDistance)
    {
        GroundNormal = Vector3.Zero;
        SkyDirection = Vector3.Zero;
        GroundPoint = Vector3.Zero;
        _groundPointWeightTotal = 0;

        _origin = queryOrigin;
        _raycastDist = raycastDistance;
    }


    public void Finalise()
    {
        SkyDirection = SkyDirection.Normalized();
        GroundNormal = GroundNormal.Normalized();
        GroundPoint /= _groundPointWeightTotal;
        GroundPoint += _origin;
        GroundHeight = (GroundPoint - _origin).Length() - ObjectRadius;
    }
}