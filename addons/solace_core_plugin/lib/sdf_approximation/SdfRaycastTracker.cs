using Godot;
using Godot.Collections;

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
    public bool HasSavedHit => HasNaturalHit || HasTrackerHit;
    public bool HasSavedMiss => !HasNaturalHit || !HasTrackerHit;

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
        // execute a raycast in the natural direction to get space state.
        var raycastEndPoint = queryOrigin + (RaycastDirection * raycastDistance);

        if (HasSavedMiss && HasNaturalHit)
        {
            // do a raycast to the missed spot to "Track that empty area"
            var trackerTarget = MissPosition;
            SetupTrackingRaycast(queryOrigin, raycastEndPoint, trackerTarget, raycastDistance);
            HasTrackerHit = ExecuteRaycast(spaceState);
        }
        else if (HasSavedHit && !HasNaturalHit)
        {
            // do a raycast to the hit spot to "Track that full area"
            var trackerTarget = HitPosition;
            SetupTrackingRaycast(queryOrigin, raycastEndPoint, trackerTarget, raycastDistance);
            HasTrackerHit = ExecuteRaycast(spaceState);
        }

        // do a natural raycast to get the exact state of the assigned direction.
        // result of the natural raycast overwrites the tracking result.
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
        }
        else
        {
            MissPosition = _queryParameters.To;
        }

        return hasHit;
    }

    /// <summary>
    /// Sets up the parameters for 'tracking raycast';
    /// Projects the given target to the current 'raycast sphere',
    /// And steps towards the 'natural' position over time.
    /// </summary>
    /// <param name="queryOrigin">Center of the raycast sphere, in global coordinates</param>
    /// <param name="raycastEndPoint">The 'natural' end point for the raycast, global coordinates.</param>
    /// <param name="positionToTrack">Global position of the desired location to track.</param>
    /// <param name="maxLength">The maximum raycast allowed for tracking.</param>
    private void SetupTrackingRaycast(
        Vector3 queryOrigin,
        Vector3 raycastEndPoint,
        Vector3 positionToTrack,
        float maxLength
    )
    {
        var oldTargetDirection = (positionToTrack - queryOrigin).Normalized();
        var oldTargetProjected = queryOrigin + (oldTargetDirection * maxLength);
        var pointIterationDirection = (raycastEndPoint - oldTargetProjected);
        var trackedTargetGlobalPosition = oldTargetProjected + (pointIterationDirection.Normalized() * 0.1f);
        var newCastDirection = trackedTargetGlobalPosition - queryOrigin;
        _queryParameters.From = queryOrigin;
        _queryParameters.To = newCastDirection;
    }
}