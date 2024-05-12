using Godot;

namespace Solace.addons.solace_core_plugin.lib.utilities;

public static class RangeUtilities
{
    /// <summary>
    /// Converts from 0~max range to 0~1 range linearly.
    /// Values outside the range are clamped to 0 or 1.
    /// </summary>
    /// <param name="current">Value expected to fit between 0~max </param>
    /// <param name="max">Max value expected.</param>
    /// <returns>0~1; max distance is treated as 1</returns>
    public static float ExactRange01(float current, float max)
    {
        return max > float.Epsilon
            ? (current / max).Clamp01()
            : 0f;
    }

    /// <summary>
    /// Converts from 0~max range to 1~0 range linearly.
    /// Values outside the range are clamped to 0 or 1.
    /// </summary>
    /// <param name="current">Value expected to fit between 0~max</param>
    /// <param name="max">Max value expected.</param>
    /// <returns>0~1; max distance is treated as 0.</returns>
    public static float ExactRange01Inverted(float current, float max)
    {
        return 1 - ExactRange01(current, max);
    }

    /// <summary>
    /// Converts two amounts to a ratio between 0~1 linearly.
    /// Values outside the range are clamped to 0 or 1.
    /// </summary>
    /// <param name="includedValue">Included part of the ratio</param>
    /// <param name="remainingValue">Excluded part of the ratio</param>
    /// <returns>0~1; ratio of included to total</returns>
    public static float RatioRange01(float includedValue, float remainingValue)
    {
        var sum = includedValue + remainingValue;
        return sum > float.Epsilon
            ? (includedValue / sum).Clamp01()
            : 0f;
    }


    /// <summary>
    /// Converts from [0,max] range to [~0,1] range exponentially.
    /// Values outside this range approach 0.
    /// Therefore {-inf,0,max,inf} maps to {0,~0,~1,0}
    /// Approximate zero ~0 depends on smoothing factor.
    /// </summary>
    /// <param name="current">Value expected to fit between 0~max </param>
    /// <param name="max">Max value expected.</param>
    /// <param name="smoothing">How small the approximate zero is; 1~8 recommended.</param>
    /// <returns>0~1; max distance is treated as ~1</returns>
    public static float WeightedRange01(float current, float max, int smoothing = 8)
    {
        var ratio = max > float.Epsilon
            ? current / max
            : 0f;
        return ratio.DecayFrom1(smoothing);
    }


    /// <summary>
    /// Converts from [0,max] range to [1,~0] range exponentially.
    /// Values outside this range approach 0.
    /// Therefore {-inf,0,max,inf} maps to {0,1,~0,0}
    /// Approximate zero ~0 depends on smoothing factor.
    /// </summary>
    /// <param name="current">Value expected to fit between 0~max</param>
    /// <param name="max">Max value expected.</param>
    /// <param name="smoothing">How small the approximate zero is; 1~8 recommended.</param>
    /// <returns>0~1; max distance is treated as ~0.</returns>
    public static float WeightedRange01Inverted(float current, float max, int smoothing = 8)
    {
        var ratio = max > float.Epsilon
            ? current / max
            : 0f;
        return ratio.DecayFrom0(smoothing);
    }
}