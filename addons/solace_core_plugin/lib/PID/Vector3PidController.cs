using Godot;

namespace Solace.addons.solace_core_plugin.lib.PID;

/// <summary>
/// Proportional–integral–derivative controller
/// </summary>
public class Vector3PidController
{
    private Vector3 StoredIntegral { get; set; } = Vector3.Zero;
    public Vector3 Result { get; private set; }


    public void Reset()
    {
        StoredIntegral = Vector3.Zero;
    }

    /// <param name="gain">P,I,D gain; followed by integral max.</param>
    /// <param name="error">Target - current value</param>
    /// <param name="velocity">Current velocity</param>
    /// <param name="dt">Time difference</param>
    public void Update(Vector4 gain, Vector3 error, Vector3 velocity, double dt)
    {
        var proportional = gain.X * error;
        StoredIntegral = (StoredIntegral + error * (float)dt).LimitLength(gain.W);
        var integral = gain.Y * StoredIntegral;
        var derivative = -gain.Z * velocity;

        Result = proportional + integral + derivative;
    }

    public void SetFrequencyAndDamping(ref Vector4 gain, float frequency, float damping)
    {
        // proportional
        gain = gain with { X = 6f * frequency * (6f * frequency) * 0.25f };
        // derivative
        gain = gain with { Z = 4.5f * frequency * damping };
    }
}