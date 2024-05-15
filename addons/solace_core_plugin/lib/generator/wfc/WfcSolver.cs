namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

/// <summary>
/// Handles constraint solving steps
/// </summary>
public class WfcSolver
{
    private WfcCell[] cells;
    private WfcPropagator _propagator;


    private void Setup()
    {
    }

    private void StepConstraintSolve()
    {
        _propagator.PopMinEntropy(cells);

        _propagator.PropagateChanges();

        // TODO: is the collapse check done on the propagator or the layer?
        // How do we handle multiple layers?
        
        
        // _propagator.CheckCollapsed();
        // if (_propagator.isCollapsed)
        // {
        //     Finalise();
        // }
    }


    private bool CheckCollapsed()
    {
        return false;
    }





    private void Finalise()
    {
    }
}