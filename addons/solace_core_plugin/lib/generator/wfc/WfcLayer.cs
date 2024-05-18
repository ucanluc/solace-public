using System.Collections.Generic;

namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

/// <summary>
/// Defines a combination of <see cref="WfcOption"/>s.
/// May target another layer to constrain.
/// </summary>
public class WfcLayer
{
    private string _layerName;
    private WfcOption[] _options;
    
    public bool HasIndividualEntropy { get; }

    /// <summary>
    /// Weight of each option for being selected,
    /// Only used if option-individual entropy is enabled.
    /// </summary>
    public float[] OptionWeights { get; }

    /// <summary>
    /// Weight of this layer;
    /// Only used directly if option-individual entropy is disabled.
    /// </summary>
    public float LayerWeight { get; }

    /// <summary>
    /// Constrain the layer at given offset; by the
    /// 'index of the choice on this layer' => 'choice whitemask on other layer'.
    /// </summary>
    public Dictionary<int, ulong[]> LayerTargetedWhitelistsByOffset { get; }


    /// <summary>
    /// 6 directions; select the whitemask array.
    /// 'index of the choice on this wave' => 'choice whitemask on neighbouring wave'.
    /// Directions go -X, -Y, -Z, X, Y, Z.
    /// </summary>
    public ulong[][] WhitelistsByDir { get; }
}