using System;
using System.Collections.Generic;

namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

public class WfcEntropyTracker
{
    private readonly GroupedTracker _groupedTracker = new GroupedTracker();
    private readonly WfcMap _map;

    public WfcEntropyTracker(WfcMap map)
    {
        _map = map;
    }


    public bool IsCollapsed => _groupedTracker.IsEmpty;


    public int PopMinEntropy()
    {
        var selectedWave = _groupedTracker.GetRandomMin();
        _map.CollapseWave(selectedWave);
        UpdateWave(selectedWave);
        return selectedWave;
    }

    /// <summary>
    /// Clear tracker status and track all active waves in given map.
    /// </summary>
    /// <param name="map">Map to track waves on</param>
    public void RestartTracking()
    {
        _groupedTracker.Clear();
        foreach (var waveId in _map.GetActiveWaves())
        {
            var entropy = _map.GetWaveEntropy(waveId);
            _groupedTracker.TrackItem(waveId, entropy);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="waveId"></param>
    /// <param name="map"></param>
    public void UpdateWave(int waveId)
    {
        var waveCanBeTracked = _map.IsWaveActive(waveId);
        if (!waveCanBeTracked)
        {
            _groupedTracker.UntrackItem(waveId);
            return;
        }

        var currentEntropy = _map.GetWaveEntropy(waveId);
        _groupedTracker.TrackItem(waveId, currentEntropy);
    }

    public void UpdateWaves(int[] changedWaves)
    {
        foreach (var changedWave in changedWaves)
        {
            UpdateWave(changedWave);
        }
    }
}