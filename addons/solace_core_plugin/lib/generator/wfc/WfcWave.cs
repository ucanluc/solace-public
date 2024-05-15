using System;
using System.Numerics;
using Godot;

namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

/// <summary>
/// Tracks the possibility of exclusive choices.
/// Represents a 'wave of potentials' in fancy terms.
///
/// Uses bitflags to speed up comparisons and minimise memory usage.
///  
/// If only one possibility is left: that is the truth.
/// If multiple possibilities exist; they are undecided.
/// If no possibilities exist; then there is a contradiction.
/// </summary>
public struct WfcWave
{
    private const int MaxBitCount = 64;

    /// <summary>
    /// Flag; Represents 64 exclusive options
    /// Each bit set to one is a possibility.
    /// Bits set to zero are impossible.
    /// </summary>
    private ulong _possibilities;

    public int PossibilityCount => BitOperations.PopCount(_possibilities);


    /// <summary>
    /// If true; the wave has multiple possible resolutions; and is unsettled.
    /// If false; the wave's resolution state is exact, or unknowable.
    /// </summary>
    public bool IsUndecided => PossibilityCount > 1;

    /// <summary>
    /// If true; the wave only has one valid resolution; which can be assumed to be exactly true.
    /// If false; The wave's resolution is undecided or unknowable.
    /// </summary>
    public bool IsDecided => PossibilityCount == 1;

    /// <summary>
    /// when true; no possibility exists; wave's resolution is unknowable.
    /// when false; the wave's resolution is knowable.
    /// </summary>
    public bool IsContradiction => PossibilityCount == 0;

    public WfcResolutionState GetWaveState()
    {
        return BitOperations.PopCount(_possibilities) switch
        {
            0 => WfcResolutionState.Contradiction,
            1 => WfcResolutionState.Decided,
            _ => WfcResolutionState.Undecided
        };
    }


    /// <summary>
    /// Only keep possibilities given by a whitelist.
    /// Bits set to zero will are made impossible
    /// </summary>
    /// <param name="whitelistMask">Whitelist mask to constrain by</param>
    /// <returns>True if there are any changes, False if no changes were made.</returns>
    public bool ConstrainByWhitelist(ulong whitelistMask)
    {
        var previous = _possibilities;
        _possibilities &= whitelistMask;
        return (previous ^ _possibilities) > 0;
    }

    /// <summary>
    /// Allow possibilities given by a whitelist.
    /// Bits set to one are made possible
    /// </summary>
    /// <param name="whitelistMask">Whitelist mask to add potentials by</param>
    /// <returns>True if there are any changes, False if no changes were made.</returns>
    public bool AddPotentialsByWhitelist(ulong whitelistMask)
    {
        var previous = _possibilities;
        _possibilities |= whitelistMask;
        return (previous ^ _possibilities) > 0;
    }


    /// <summary>
    /// Get the status of a single possibility
    /// </summary>
    /// <param name="index">Index of the possibility to check</param>
    /// <returns>True if possible, false if impossible.</returns>
    public bool GetSinglePossibility(int index)
    {
        return (_possibilities & BitFlag(index)) > 0;
    }

    /// <summary>
    /// Removes a single possibility from this wave.
    /// </summary>
    /// <param name="index">Index of the possibility to remove.</param>
    /// <returns>True if there are any changes, False if no changes were made.</returns>
    public bool RemoveSinglePossibility(int index)
    {
        var previous = _possibilities;
        _possibilities = (_possibilities & ~BitFlag(index));
        return (previous ^ _possibilities) > 0;
    }

    /// <summary>
    /// Allows a single possibility to this wave.
    /// </summary>
    /// <param name="index">Index of the possibility to include</param>
    /// <returns>True if there are any changes, False if no changes were made.</returns>
    public bool AddSinglePossibility(int index)
    {
        var previous = _possibilities;
        _possibilities = (_possibilities | BitFlag(index));
        return (previous ^ _possibilities) > 0;
    }

    /// <summary>
    /// Get the bitflag at given index.
    /// </summary>
    /// <param name="index">How much to shift the bit to the left</param>
    /// <returns>ulong bitflag at given index</returns>
    private static ulong BitFlag(int index)
    {
        //TODO:`(ulong)1<<i` may preserve the bits; needs testing.
        const ulong one = 1;
        return one << index;
    }

    /// <summary>
    /// Gets entropy of the current possibility space, using the different weights for each existing possibility.
    /// 
    /// Lower entropy = Less choices exist and/or some possibilities are very likely = Easier to decide.
    /// Higher entropy = More choices exist and/or possibilities are equally likely = Harder to decide.
    /// </summary>
    /// <param name="possibilityWeights"></param>
    /// <returns>The entropy of this wave</returns>
    public float GetWeightedWaveEntropy(in float[] possibilityWeights)
    {
        float weightSum = 0, weightLogSum = 0;
        // TODO: There's probably an O(1) approach to this.
        for (var i = 0; i < possibilityWeights.Length; i++)
        {
            if (!GetSinglePossibility(i)) continue;
            var weight = possibilityWeights[i];
            weightSum += weight;
            weightLogSum += weight * Mathf.Log(weight);
        }

        var cellEntropy = Mathf.Log(weightSum) - (weightLogSum / weightSum);
        return cellEntropy;
    }

    /// <summary>
    /// Gets entropy of the current possibility space, using the same weight for each existing possibility.
    /// 
    /// Lower entropy = Less choices exist and/or some possibilities are very likely = Easier to decide.
    /// Higher entropy = More choices exist and/or possibilities are equally likely = Harder to decide.
    /// </summary>
    /// <param name="defaultPossibilityWeight">Default weight to use for all possibilities</param>
    /// <returns>The entropy of this wave</returns>
    public float GetDefaultWaveEntropy(in float defaultPossibilityWeight)
    {
        var weightSum = PossibilityCount * defaultPossibilityWeight;
        var weightLogSum = PossibilityCount * (defaultPossibilityWeight * Mathf.Log(defaultPossibilityWeight));

        var cellEntropy = Mathf.Log(weightSum) - (weightLogSum / weightSum);
        return cellEntropy;
    }

    /// <summary>
    /// Create an inclusive whitelist; for existing possibilities.
    /// Possibility lookups are based on the index of the whitelist.
    /// </summary>
    /// <param name="whitelists">Possibility Whitelists, indexed by the enabling possibility</param>
    /// <returns>Inclusively combined whitelist.</returns>
    public ulong GetCompositeWhitelist(ulong[] whitelists)
    {
        ulong compositeWhitelist = 0;
        // TODO: There's probably an O(1) approach to this.
        for (var i = 0; i < whitelists.Length; i++)
        {
            if (!GetSinglePossibility(i)) continue;
            compositeWhitelist |= whitelists[i];
        }

        return compositeWhitelist;
    }

    /// <summary>
    /// Force selecting a single possibility.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void ForceDecide()
    {
        throw new NotImplementedException();
    }
}