using System.Collections.Generic;

namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

/// <summary>
/// Defines an option which has directional and internal <see cref="WfcSocket"/>s, 
/// Options defined in a <see cref="WfcLayer"/> will be reduced down to one per <see cref="WfcCell"/>    
/// </summary>
public class WfcOption
{
    /// <summary>
    /// Defines the 'edge' surfaces of this tile option.
    /// </summary>
    private WfcSocket[] _directionalSockets;

    /// <summary>
    /// Defines inherent qualities about this option; may be used for validation between layers, or pre/postprocessing.
    /// </summary>
    private Dictionary<int, WfcSocket> _internalSockets;

    /// <summary>
    /// Relative weight for prioritizing this option over others in the same layer.
    /// </summary>
    public float SelectionWeight;

    public WfcOption(WfcSocket[] directionalSockets, Dictionary<int, WfcSocket> internalSockets, float selectionWeight)
    {
        _directionalSockets = directionalSockets;
        _internalSockets = internalSockets;
        SelectionWeight = selectionWeight;
    }
}