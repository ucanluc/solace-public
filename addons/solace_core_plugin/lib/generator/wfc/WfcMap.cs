using System;
using System.Collections.Generic;
using Godot;
using Solace.addons.solace_core_plugin.core;
using Solace.addons.solace_core_plugin.lib.utilities;

namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

public class WfcMap
{
    private readonly WfcLayer[] _layers;
    private readonly WfcCell[] _cells;

    private readonly WfcWave[] _waves;
    private readonly Vector4I _dimensions;
    private readonly Vector3I _mapSize;

    /// <summary>
    /// Wave ids are indexed over: Layer, X,Y,Z
    /// </summary>
    private readonly List<int> _activeWaveIds;

    public WfcMap(Vector3I mapSize, WfcLayer[] layers)
    {
        _layers = layers;
        _mapSize = mapSize;

        if ((double)mapSize.X * mapSize.Y * mapSize.Z * layers.Length > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(mapSize),
                $"The wave count required to represent the map exceeds {nameof(int.MaxValue)}");
        }

        _dimensions = new Vector4I(layers.Length, mapSize.X, mapSize.Y, mapSize.Z);

        var cellCount = mapSize.X * mapSize.Y * mapSize.Z;
        var waveCount = cellCount * layers.Length;
        _waves = new WfcWave[waveCount];
        _cells = new WfcCell[cellCount];
        _activeWaveIds = new List<int>(waveCount);

        for (var index = 0; index < _waves.Length; index++)
        {
            _activeWaveIds[index] = index;
        }
    }

    private int GetWaveLayerIndex(int waveId)
    {
        return waveId % _layers.Length;
    }

    public float GetWaveEntropy(int waveId)
    {
        var wave = _waves[waveId];
        var layer = _layers[GetWaveLayerIndex(waveId)];
        var isWeighted = layer.HasIndividualEntropy;
        return isWeighted
            ? wave.GetWeightedWaveEntropy(layer.OptionWeights)
            : wave.GetDefaultWaveEntropy(layer.LayerWeight);
    }

    /// <summary>
    /// Apply the constraints of this wave to its neighbours and aligned waves on other layers.
    /// </summary>
    /// <param name="waveId"></param>
    /// <returns></returns>
    public int[] PropagateConstraints(int waveId)
    {
        var adjacentWaves = GetComparisons(waveId, out var whitelists);

        var changedWaves = new List<int>();
        var currentWave = _waves[waveId];
        for (var index = 0; index < adjacentWaves.Length; index++)
        {
            var waveToConstrain = adjacentWaves[index];
            var targetWave = _waves[waveToConstrain];
            var allowMask = currentWave.GetCompositeWhitelist(whitelists[index]);
            var hasChanges = targetWave.ConstrainByWhitelist(allowMask);
            if (hasChanges)
            {
                changedWaves.Add(waveToConstrain);
            }
        }

        return changedWaves.ToArray();
    }

    private int[] GetComparisons(int waveId, out ulong[][] whitelists)
    {
        var currentCoordinates = waveId.ToDimensionalCoordinates(_dimensions);
        var offsets = GetNeighbourOffsets(waveId, currentCoordinates, out whitelists);
        var targetWaves = new int[offsets.Length];
        for (var i = 0; i < offsets.Length; i++)
        {
            var offset = offsets[i];
            targetWaves[i] = (currentCoordinates + offset).ToDimensionalIndex(_dimensions);
        }

        return targetWaves;
    }

    private Vector4I[] GetNeighbourOffsets(int waveId, Vector4I currentCoordinates, out ulong[][] whitelists)
    {
        var neighbourOffsets = new List<Vector4I>();

        _dimensions.Deconstruct(out var dX, out var dY, out var dZ, out var dW);
        currentCoordinates.Deconstruct(out var x, out var y, out var z, out var w);
        var currentLayerIndex = GetWaveLayerIndex(waveId);
        var currentLayer = _layers[currentLayerIndex];

        var tempWhitelists = new List<ulong[]>();
        // get neighbours in 3d space
        // TODO: cleanup directional indexing.
        if (0 < y)
        {
            neighbourOffsets.Add(new Vector4I(0, -1, 0, 0));
            tempWhitelists.Add(currentLayer.WhitelistsByDir[0]);
        }

        if (0 < z)
        {
            neighbourOffsets.Add(new Vector4I(0, 0, -1, 0));
            tempWhitelists.Add(currentLayer.WhitelistsByDir[1]);
        }

        if (0 < w)
        {
            neighbourOffsets.Add(new Vector4I(0, 0, 0, -1));
            tempWhitelists.Add(currentLayer.WhitelistsByDir[2]);
        }

        if (y < dY - 1)
        {
            neighbourOffsets.Add(new Vector4I(0, 1, 0, 0));
            tempWhitelists.Add(currentLayer.WhitelistsByDir[3]);
        }

        if (z < dZ - 1)
        {
            neighbourOffsets.Add(new Vector4I(0, 0, 1, 0));
            tempWhitelists.Add(currentLayer.WhitelistsByDir[4]);
        }

        if (w < dW - 1)
        {
            neighbourOffsets.Add(new Vector4I(0, 0, 0, 1));
            tempWhitelists.Add(currentLayer.WhitelistsByDir[5]);
        }


        // get the other layers for comparing to:
        foreach (var layerOffset in currentLayer.LayerTargetedWhitelistsByOffset)
        {
            neighbourOffsets.Add(new Vector4I(0, 0, 0, layerOffset.Key));
            tempWhitelists.Add(layerOffset.Value);
        }

        whitelists = tempWhitelists.ToArray();

        return neighbourOffsets.ToArray();
    }

    public bool IsWaveActive(int waveId) => _activeWaveIds.Contains(waveId);
    public int[] GetActiveWaves() => _activeWaveIds.ToArray();

    public bool CollapseWave(int waveId)
    {
        var wave = _waves[waveId];
        var layer = _layers[GetWaveLayerIndex(waveId)];
        var isWeighted = layer.HasIndividualEntropy;

        return isWeighted
            ? wave.ForceDecide(layer.OptionWeights)
            : wave.ForceDecide();
    }
}