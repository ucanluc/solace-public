using System;
using Godot;
using Solace.addons.solace_core_plugin.lib.utilities;

namespace Solace.addons.solace_core_plugin.lib.sdf_approximation;

public class SdfSnapshot
{
    /// <summary>
    /// Optional; Ignores secondary/cached values, only using the immediately accessible data. 
    /// </summary>
    public bool IgnoreArchived { private get; set; }

    public bool DrawDebugLines { private get; set; }

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
    public Vector3 WeightFitHitNormal { private get; set; }

    /// <summary>
    /// Optional; set to prioritise hits in the given direction.
    /// If length = 0; Disabled
    /// If length is 0~1; Mild adjustment
    /// If length is 1; Weights are between 0~1 depending on alignment. 
    /// If length above 1; Starts ignoring misaligned vectors completely. 
    /// </summary>
    public Vector3 WeightFitDirection { private get; set; }


    /// <summary>
    /// Optional; set to align surface normal with the given direction.
    /// If length = 0; Disabled
    /// If length is 0~2; Length decides alignment strength. 
    /// If length is 1 or above; Force assume this as the surface normal. 
    /// </summary>
    public Vector3 WeightFitSurfaceNormal { private get; set; }

    /// <summary>
    /// Derived output:
    /// This direction is "UP"; If you want to stay parallel to the surface.
    /// According to nearby surface normals.
    /// This is the closest value to the main output of a Signed Distance field; if we were using one.
    /// </summary>
    public Vector3 SurfaceNormal { get; private set; }

    /// <summary>
    /// Derived output:
    /// This point in global space is the most distant from known objects.
    /// Aka. the assumed center of the nearby "sky volume" in global space.
    /// </summary>
    public Vector3 CompositeSkyPosition { get; private set; }

    /// <summary>
    /// Derived output;
    /// This point in global space is the closest to known objects.
    /// Aka. the assumed center of the "ground plane" in global space
    /// </summary>
    public Vector3 CompositeGroundPosition { get; private set; }

    /// <summary>
    /// Derived output;
    /// This point is the center of the known ground volume, aligned with the current surface normal.
    /// Also in global space.
    /// </summary>
    public Vector3 ProjectedGroundPosition { get; private set; }

    /// <summary>
    /// Derived output;
    /// This point is the center of the known sky volume, aligned with the current surface normal.
    /// Also in global space.
    /// </summary>
    public Vector3 ProjectedSkyPosition { get; private set; }

    /// <summary>
    /// Derived output;
    /// This is the movement/translation vector that centers us with the most ground,
    /// Is parallel to the ground normal, and is in global space.
    /// </summary>
    public Vector3 GroundAligningMovement { get; private set; }

    /// <summary>
    /// Derived output;
    /// This is the movement/translation vector that centers us with the most sky,
    /// Is parallel to the ground normal, and is in global space.
    /// </summary>
    public Vector3 SkyAligningMovement { get; private set; }

    /// <summary>
    /// Derived output;
    /// This is our height from the assumed ground plane.
    /// May be negative if we are below the assumed ground plane;
    /// Already includes object radius.
    /// </summary>
    public float DistanceToGround { get; private set; }

    /// <summary>
    /// Derived output;
    /// This is our height from the assumed sky center-plane.
    /// May be negative if we are above the assumed center-plane
    /// Already includes object radius.
    /// </summary>
    public float DistanceToSky { get; private set; }

    /// <summary>
    /// Derived output;
    /// 0~1 how far away we are from ground; as a ratio of total sky-ground distance.
    /// </summary>
    public float HeightRatio { get; private set; }

    private float _trackerHitsWeightTotal, _trackerMissesWeightTotal, _raycastDist;
    private Vector3 _origin;

    /// <summary>
    /// Derives weighted values from the given tracker's status.
    /// </summary>
    /// <param name="tracker">Tracker to read from.</param>
    public void IntegrateTracker(SdfRaycastTracker tracker)
    {
        // calculate the base variables


        var missWeight = IntegrateTrackerMiss(tracker);

        var hitWeight = IntegrateTrackerHit(tracker);

        if (DrawDebugLines) DrawTrackerDebug(tracker, hitWeight, missWeight);
    }


    private float IntegrateTrackerMiss(SdfRaycastTracker tracker)
    {
        if (IgnoreArchived && !tracker.HasNaturalHit)
        {
            // Ignored due to not being new.
            return 0;
        }

        var trackerMissPosition = tracker.MissPosition;
        if (tracker.HasArchivedMiss)
        {
            // The distance until this point is known to be clear.
            trackerMissPosition = tracker.HitPosition;
        }

        var maxDistance = Mathf.Clamp(_raycastDist - ObjectRadius, 0, _raycastDist);

        var missVector = (trackerMissPosition - _origin);
        var missDistance = Mathf.Clamp(missVector.Length() - ObjectRadius, 0, maxDistance);
        var missDirection = missVector.Normalized();

        // Distant hits are more important for tracking the sky
        var skyWeight = RangeUtilities.WeightedRange01(missDistance, maxDistance);

        // Optional; Directional fit; prefers misses in given direction.
        var dirFitWeight =
            WeightFitDirection.Length() > float.Epsilon
                ? VectorUtilities.Dot01(missDirection, WeightFitDirection)
                : 1;

        skyWeight *= dirFitWeight * dirFitWeight;

        CompositeSkyPosition += missVector * skyWeight;
        _trackerMissesWeightTotal += skyWeight;
        return skyWeight;
    }

    /// <summary>
    /// Update the derived values from the tracker hit data.
    /// weights are exponential akin to "Least squares fitting";
    /// https://mathworld.wolfram.com/LeastSquaresFitting.html
    /// </summary>
    /// <param name="tracker">Tracker to read from</param>
    /// <returns>The importance weight of the tracker hit; between 0~1.</returns>
    private float IntegrateTrackerHit(SdfRaycastTracker tracker)
    {
        if (IgnoreArchived && !tracker.HasNaturalHit)
        {
            // Ignored due to not being new.
            return 0;
        }

        // hit vectors are integrated with the origin as a basis, for weighted averaging.
        var maxDistance = Mathf.Clamp(_raycastDist - ObjectRadius, 0, _raycastDist);
        var hitVector = (tracker.HitPosition - _origin);
        var hitDistance = Mathf.Clamp(hitVector.Length() - ObjectRadius, 0, maxDistance);
        var hitNormal = tracker.HitNormal.Normalized();
        var hitDirection = hitVector.Normalized();

        // Closer hits are more important for tracking the ground.
        var groundWeight = RangeUtilities.WeightedRange01Inverted(hitDistance, maxDistance);

        // Optional; Normal fit; prefers hits with aligned normals. 
        var normalFitWeight =
            WeightFitHitNormal.Length() > float.Epsilon
                ? VectorUtilities.Dot01(hitNormal, WeightFitHitNormal)
                : 1;

        // Optional; Directional fit; prefers hits in given direction.
        var dirFitWeight =
            WeightFitDirection.Length() > float.Epsilon
                ? VectorUtilities.Dot01(hitDirection, WeightFitDirection)
                : 1;

        normalFitWeight *= normalFitWeight;
        dirFitWeight *= dirFitWeight;

        // Combine and integrate the weights to relevant data.
        var trackerWeight = groundWeight * dirFitWeight * normalFitWeight;
        SurfaceNormal += hitNormal * trackerWeight;
        CompositeGroundPosition += hitVector * trackerWeight;
        _trackerHitsWeightTotal += trackerWeight;

        return trackerWeight;
    }

    private static void DrawTrackerDebug(SdfRaycastTracker tracker, float hitWeight, float missWeight)
    {
        if (tracker.HasArchivedHit) tracker.DrawPosition(tracker.HitPosition, Colors.White, hitWeight);
        if (tracker.HasArchivedMiss) tracker.DrawPosition(tracker.MissPosition, Colors.Black, missWeight);

        if (!tracker.HasNaturalHit)
        {
            if (tracker.HasSavedHit) tracker.DrawPosition(tracker.HitPosition, Colors.Yellow, hitWeight);
            if (tracker.HasSavedMiss) tracker.DrawPosition(tracker.MissPosition, Colors.Red, missWeight);
        }
        else
        {
            if (tracker.HasSavedHit) tracker.DrawPosition(tracker.HitPosition, Colors.Green, hitWeight);
            if (tracker.HasSavedMiss) tracker.DrawPosition(tracker.MissPosition, Colors.Blue, missWeight);
        }
    }


    /// <summary>
    /// Clear the saved snapshot data, and prepare for integrating new data.
    /// </summary>
    /// <param name="queryOrigin">Global position of the snapshot location</param>
    /// <param name="raycastDistance">Distance to use as a reference while integrating.</param>
    public void Clear(Vector3 queryOrigin, float raycastDistance)
    {
        SurfaceNormal = Vector3.Zero;
        CompositeGroundPosition = Vector3.Zero;
        CompositeSkyPosition = Vector3.Zero;
        _trackerHitsWeightTotal = 0;
        _trackerMissesWeightTotal = 0;

        _origin = queryOrigin;
        _raycastDist = raycastDistance;
    }


    /// <summary>
    /// Does the postprocessing necessary for the derived data to become useful.
    /// </summary>
    public void Finalise()
    {
        FinaliseIntegrations();
        UpdateDerivedValues();

        if (DrawDebugLines) DrawDebug();
    }

    /// <summary>
    /// Create values derived from integrated values.
    /// </summary>
    private void UpdateDerivedValues()
    {
        DistanceToGround = _origin.DecomposeWithPlane(
            CompositeGroundPosition, SurfaceNormal,
            out var skyHeightVector, out var skyTranslationVector
        );

        ProjectedGroundPosition = _origin - skyHeightVector;
        GroundAligningMovement = -skyTranslationVector;

        DistanceToSky = _origin.DecomposeWithPlane(
            CompositeSkyPosition, -SurfaceNormal,
            out var groundHeightVector, out var groundTranslationVector
        );

        ProjectedSkyPosition = _origin - groundHeightVector;
        SkyAligningMovement = -groundTranslationVector;

        HeightRatio = RangeUtilities.RatioRange01(DistanceToGround, DistanceToSky);
    }

    /// <summary>
    /// Finalise sums 
    /// </summary>
    private void FinaliseIntegrations()
    {
        FinaliseNormal();

        CompositeGroundPosition /= _trackerHitsWeightTotal;
        CompositeSkyPosition /= _trackerMissesWeightTotal;

        CompositeGroundPosition += _origin;
        CompositeSkyPosition += _origin;
        return;

        void FinaliseNormal()
        {
            var normalRefitWeight = WeightFitSurfaceNormal.Length().Clamp01();
            switch (normalRefitWeight)
            {
                case 1:
                    SurfaceNormal = WeightFitSurfaceNormal;
                    break;
                case > 0 when SurfaceNormal.Length() > 0:
                {
                    var target = WeightFitSurfaceNormal.Normalized();
                    var from = SurfaceNormal.Normalized();
                    SurfaceNormal = from.Lerp(target, normalRefitWeight * target.Dot01(from));
                    break;
                }
                case > 0:
                    SurfaceNormal = WeightFitSurfaceNormal;
                    break;
            }

            SurfaceNormal = SurfaceNormal.Normalized();
        }
    }

    private void DrawDebug()
    {
        DebugDraw3D.DrawLine(CompositeSkyPosition, ProjectedSkyPosition, Colors.Blue);
        DebugDraw3D.DrawLine(CompositeGroundPosition, ProjectedGroundPosition, Colors.Red);
        DebugDraw3D.DrawLine(_origin, ProjectedGroundPosition, Colors.Yellow);
        DebugDraw3D.DrawLine(_origin, ProjectedSkyPosition, Colors.Cyan);
    }
}