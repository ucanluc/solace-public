using System;
using System.Collections.Generic;
using Godot;

namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

/// <summary>
/// Does constraint handling by tracking dirty cells.
/// </summary>
public class WfcPropagator
{
    private readonly Stack<WfcCell> _dirtyCellStack = new Stack<WfcCell>();

    /// <summary>
    /// Propagates constraints while there are cells with internal changes.
    /// </summary>
    public void PropagateChanges()
    {
        while (_dirtyCellStack.TryPop(out var cell))
        {
            var dirtyCells = cell.PropagateConstraints();
            foreach (var dirtyCell in dirtyCells)
            {
                if (_dirtyCellStack.Contains(dirtyCell)) continue;
                _dirtyCellStack.Push(dirtyCell);
            }
        }
    }

    public void AddDirty(WfcCell dirtyCell)
    {
        _dirtyCellStack.Push(dirtyCell);
    }

    public void AddDirty(WfcCell[] dirtyCells)
    {
        foreach (var dirtyCell in dirtyCells)
        {
            _dirtyCellStack.Push(dirtyCell);
        }
    }

    /// <summary>
    /// Pop one of the cells with the lowest entropy to create something to propagate.
    /// </summary>
    public void PopMinEntropy(WfcCell[] cells)
    {
        throw new NotImplementedException();
        // TODO: updating entropy can be done lazily; 
        
        
        // Reset();
        // foreach (var cell in cells)
        // {
        //     if (cell.IsDecided)
        //     {
        //         continue;
        //     }
        //
        //     cell.GetWaveEntropy(cell);
        // }
        //
        // var cellToPop = GetRandomMinEntropyCell();
        //
        // cellToPop.PopRandom();
        //
        // _dirtyCellStack.Push(cellToPop);
    }
}