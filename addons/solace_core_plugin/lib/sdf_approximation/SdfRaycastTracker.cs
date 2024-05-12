using Godot;
using Godot.Collections;
using Solace.addons.solace_core_plugin.lib.utilities;

namespace Solace.addons.solace_core_plugin.lib.sdf_approximation;

/// <summary>
/// Checks a dedicated angle for raycast hits, and tracks the spot with the previous state. 
/// </summary>
public class SdfRaycastTracker
{
    public readonly Vector3 RaycastDirection;
    public Vector3 MissPosition { get; private set; }
    public Vector3 HitPosition { get; private set; }
    public Vector3 HitNormal { get; private set; }

    public bool HasArchivedMiss { get; private set; }
    public bool HasArchivedHit { get; private set; }

    public bool HasSavedHit { get; private set; }
    public bool HasSavedMiss { get; private set; }

    public bool HasNaturalHit { get; private set; }
    public bool HasTrackerHit { get; private set; }

    private readonly PhysicsRayQueryParameters3D _queryParameters;

    public SdfRaycastTracker(Vector3 raycastDirection, uint mask)
    {
        RaycastDirection = raycastDirection;
        _queryParameters = PhysicsRayQueryParameters3D.Create(Vector3.Zero, raycastDirection, mask);
    }

    public void UpdateRaycast(PhysicsDirectSpaceState3D spaceState, Vector3 queryOrigin, float raycastDistance)
    {
        var raycastEndPoint = queryOrigin + (RaycastDirection * raycastDistance);

        // Tracking raycasts validate / renew saved locations
        if (HasNaturalHit)
        {
            // do a raycast to "Track the old, empty area"
            var trackerTarget = MissPosition;
            SetupTrackingRaycast(queryOrigin, trackerTarget, raycastDistance);
            HasTrackerHit = ExecuteRaycast(spaceState);
            HasSavedMiss = !HasTrackerHit;
            if (!HasSavedMiss)
            {
                HasArchivedMiss = true;
            }
        }
        else
        {
            // do a raycast to "Track the old, full area"
            var trackerTarget = HitPosition;
            SetupTrackingRaycast(queryOrigin, trackerTarget, raycastDistance);
            HasTrackerHit = ExecuteRaycast(spaceState);
            HasSavedHit = HasTrackerHit;
            if (!HasSavedHit)
            {
                HasArchivedHit = true;
            }
        }

        // do a natural raycast to get the exact state of the assigned direction.
        // result of the natural raycast overwrites the tracking result
        SetupNaturalRaycast(queryOrigin, raycastEndPoint);
        HasNaturalHit = ExecuteRaycast(spaceState);
    }

    private void SetupNaturalRaycast(Vector3 queryOrigin, Vector3 raycastEndPoint)
    {
        _queryParameters.From = queryOrigin;
        _queryParameters.To = raycastEndPoint;
    }

    private bool ExecuteRaycast(PhysicsDirectSpaceState3D spaceState)
    {
        var result = spaceState.IntersectRay(_queryParameters);
        var hasHit = result.Count > 0;
        if (hasHit)
        {
            HitPosition = (Vector3)result["position"];
            HitNormal = (Vector3)result["normal"];
            HasSavedHit = true;
            HasArchivedHit = false;
        }
        else
        {
            MissPosition = _queryParameters.To;
            HasSavedMiss = true;
            HasArchivedMiss = false;
        }

        return hasHit;
    }

    /// <summary>
    /// Sets up the parameters for 'tracking raycast';
    /// Tracking raycasts are expected to track as many 'opposites' as possible,
    /// i.e. a hit if we already know of a miss, and vice versa.
    /// especially as the known state get closer to raycasting origin.
    /// The tracking raycasts should also be evenly distributed.
    /// </summary>
    /// <param name="queryOrigin">Center of the raycast sphere, in global coordinates</param>
    /// <param name="positionToTrack">Global position of the desired location to track.</param>
    /// <param name="raycastDistance">The maximum raycast allowed for tracking.</param>
    private void SetupTrackingRaycast(
        Vector3 queryOrigin,
        Vector3 positionToTrack,
        float raycastDistance
    )
    {
        var raycastEndPoint = queryOrigin + (RaycastDirection * raycastDistance);
        var targetVector = positionToTrack - queryOrigin;
        var targetDirection = targetVector.Normalized();

        var realignmentVector = raycastEndPoint - positionToTrack;
        var realignmentDirection = realignmentVector.Normalized();

        // change between doing a realignment and exploring the area
        var iterationVector = realignmentVector
                              * realignmentDirection.Dot(targetDirection);

        var newTargetPosition = positionToTrack + iterationVector;

        // reproject point as a raycast
        var newTargetVector = (newTargetPosition - queryOrigin);
        var newTargetDirection = newTargetVector.Normalized();
        var projectedEndPoint = queryOrigin + (newTargetDirection * raycastDistance);

        _queryParameters.From = queryOrigin;
        _queryParameters.To = projectedEndPoint;
    }

    public void DrawPosition(Vector3 pointToDraw, Color color, float lineLength)
    {
        DebugDraw3D.DrawLine(pointToDraw + RaycastDirection * (lineLength),
            pointToDraw - (RaycastDirection * lineLength), color);
    }
}