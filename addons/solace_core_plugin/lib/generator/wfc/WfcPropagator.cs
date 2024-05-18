using System;
using System.Collections.Generic;


namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

/// <summary>
/// Does constraint handling by tracking dirty cells.
/// </summary>
public class WfcPropagator
{
    private readonly Stack<int> _dirtyWaveStack = new();
    private readonly WfcMap _map;

    public WfcPropagator(WfcMap map)
    {
        _map = map;
    }

    public bool HasDirty => _dirtyWaveStack.Count > 0;

    /// <summary>
    /// Propagates constraints while there are cells with internal changes.
    /// </summary>
    public int[] PropagateStep()
    {
        var tryPop = _dirtyWaveStack.TryPop(out var waveId);
        if (!tryPop) return Array.Empty<int>();

        var dirtyWaves = _map.PropagateConstraints(waveId);
        foreach (var dirtyWave in dirtyWaves)
        {
            if (_dirtyWaveStack.Contains(dirtyWave)) continue;
            _dirtyWaveStack.Push(dirtyWave);
        }

        return dirtyWaves;
    }

    public void AddDirty(int dirtyWave)
    {
        _dirtyWaveStack.Push(dirtyWave);
    }

    public void AddDirty(int[] dirtyWaves)
    {
        foreach (var dirtyCell in dirtyWaves)
        {
            _dirtyWaveStack.Push(dirtyCell);
        }
    }
}