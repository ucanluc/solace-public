using Godot;

namespace Solace.addons.solace_core_plugin.lib.utilities;

public static class FloatUtilities
{
    public static float Clamp01(this float value)
    {
        return Mathf.Clamp(value, 0, 1);
    }

    /// <summary>
    /// Maps {-inf,1,inf} ranges to {0,1,0}.
    /// given 1; returns 1.
    /// given 0 or 2; will return e^(-smoothing); which is ~0.000335 for 8 smoothing.
    /// </summary>
    /// <param name="value">Value to map from</param>
    /// <param name="smoothing">Decay rate; 1~16 recommended</param>
    /// <returns>Decay factor, between 0~1.</returns>
    public static float DecayFrom1(this float value, int smoothing)
    {
        return Mathf.Exp(-Mathf.Abs((value - 1) * smoothing));
    }

    /// <summary>
    /// Maps {-inf,0,inf} ranges to {0,1,0}.
    /// given 0; returns 1.
    /// given 1 or -1; will return e^(-smoothing); which is ~0.000335 for 8 smoothing.
    /// </summary>
    /// <param name="value">Value to map from</param>
    /// <param name="smoothing">Decay rate; 1~16 recommended</param>
    /// <returns>Decay factor, between 0~1.</returns>
    public static float DecayFrom0(this float value, int smoothing)
    {
        return Mathf.Exp(-Mathf.Abs(value * 4));
    }
}