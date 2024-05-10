using Godot;
using Solace.addons.solace_core_plugin.core;
using Solace.addons.solace_core_plugin.lib.utilities;

namespace Solace.addons.solace_core_plugin.lib.sdf_approximation;

/// <summary>
/// Approximates values that could be derived from a signed distance field.
/// </summary>
public class SdfApproximator
{
    private const int MinTrackerCount = 5;
    private readonly SdfRaycastTracker[] _trackers;
    public readonly SdfSnapshot Snapshot = new();

    public SdfApproximator(int trackerCount, uint mask)
    {
        if (trackerCount < MinTrackerCount)
        {
            SC.PrintErr(nameof(SdfApproximator), $"Given raycast tracker count is less than {MinTrackerCount}," +
                                                 $" assuming {MinTrackerCount} trackers. ");
        }


        var trackerDirections = SphereUtilities.GetPointsOnUnitSphere(trackerCount);

        _trackers = new SdfRaycastTracker[trackerCount];

        for (var index = 0; index < trackerDirections.Length; index++)
        {
            var trackerDirection = trackerDirections[index];
            _trackers[index] = new SdfRaycastTracker(trackerDirection, mask);
        }
    }

    /// <summary>
    /// Updates the SDF approximation snapshot
    /// </summary>
    /// <param name="spaceState">3D world for approximating an sdf in.</param>
    /// <param name="queryOrigin">The point to measure around in global space.</param>
    /// <param name="raycastDistance">The distance to look for objects</param>
    public void UpdateSnapshot(PhysicsDirectSpaceState3D spaceState, Vector3 queryOrigin, float raycastDistance)
    {
        Snapshot.Clear(queryOrigin, raycastDistance);

        foreach (var tracker in _trackers)
        {
            tracker.UpdateRaycast(spaceState, queryOrigin, raycastDistance);
        }


        foreach (var tracker in _trackers)
        {
            Snapshot.IntegrateTracker(tracker);
        }

        Snapshot.Finalise();
    }
}