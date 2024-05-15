using System;
using System.Collections.Generic;

namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

/// <summary>
/// Defines a volume which can only have one dedicated <see cref="WfcOption"/> when solved. 
/// </summary>
public struct WfcCell
{
    /// <summary>
    /// Defines the remaining options for this cell; defines one exact solution to this cell when resolved.
    /// </summary>
    public WfcWave wave;

    public bool IsDirty { get; private set; }
    
    public IEnumerable<WfcCell> PropagateConstraints()
    {
        throw new NotImplementedException();
    }

    public void PopRandom()
    {
        throw new NotImplementedException();
    }
}