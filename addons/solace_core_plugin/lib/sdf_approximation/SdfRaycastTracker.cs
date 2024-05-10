using Godot;

namespace Solace.addons.solace_core_plugin.lib.sdf_approximation;

public class SdfRaycastTracker
{
    public readonly Vector3 RaycastDirection;
    public Vector3 HitPosition { get; private set; }
    public Vector3 HitNormal { get; private set; }
    public bool HasCurrentHit { get; private set; }

    private readonly PhysicsRayQueryParameters3D _queryParameters;

    public SdfRaycastTracker(Vector3 raycastDirection, uint mask)
    {
        RaycastDirection = raycastDirection;
        _queryParameters = PhysicsRayQueryParameters3D.Create(Vector3.Zero, raycastDirection, mask);
    }

    public void UpdateRaycast(PhysicsDirectSpaceState3D spaceState, Vector3 queryOrigin, float raycastDistance)
    {
        _queryParameters.From = queryOrigin;
        _queryParameters.To = queryOrigin + (RaycastDirection * raycastDistance);
        var result = spaceState.IntersectRay(_queryParameters);

        HasCurrentHit = result.Count > 0;
        if (HasCurrentHit)
        {
            HitPosition = (Vector3)result["position"];
            HitNormal = (Vector3)result["normal"];
        }
    }
}