using System;
using System.Collections.Generic;

namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

/// <summary>
/// Handles constraint solving steps
/// </summary>
public class WfcSolver
{
    private readonly WfcPropagator _propagator;
    private readonly WfcEntropyTracker _entropyTracker;
    private readonly WfcMap _map;


    public WfcSolver(WfcSolverParameters parameters)
    {
        _map = new WfcMap(parameters.Dimensions, parameters.GetLayers());
        _entropyTracker = new WfcEntropyTracker(_map);
        _propagator = new WfcPropagator(_map);
    }

    private void Setup()
    {
        _entropyTracker.RestartTracking();
    }

    private void StepConstraintSolve()
    {
        if (_entropyTracker.IsCollapsed)
        {
            Finalise();
            return;
        }

        if (!_propagator.HasDirty)
        {
            var selectedWave = _entropyTracker.PopMinEntropy();
            _propagator.AddDirty(selectedWave);
        }
        else
        {
            var changedWaves = _propagator.PropagateStep();
            _entropyTracker.UpdateWaves(changedWaves);
        }
    }


    private void Finalise()
    {
    }
}