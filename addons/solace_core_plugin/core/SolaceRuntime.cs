using Solace.addons.solace_core_plugin.core.debug;

namespace Solace.addons.solace_core_plugin.core;

/// <summary>
///  Solace Core's runtime singleton; attaches into Godot's lifecycles.
/// </summary>
public class SolaceRuntime
{
    #region Singleton

    private SolaceRuntime()
    {
        Instance = this;
        SC.Print("New instance constructed.", SolacePrint.PrintType.Meta);
    }

    public static SolaceRuntime Instance { get; private set; } = new();

    public static bool Active { get; private set; }


    public static void Activate()
    {
        if (Active)
        {
            SC.Print("The instance is already active.", SolacePrint.PrintType.Meta);
            return;
        }

        Active = true;
    }


    public static void Deactivate()
    {
        if (!Active)
        {
            SC.Print("The instance is already deactivated.", SolacePrint.PrintType.Meta);
        }

        Active = false;
        SC.Print("The instance is deactivated.", SolacePrint.PrintType.Meta);
    }

    ~SolaceRuntime()
    {
        Deactivate();
        SC.Print("The instance is garbage collected.", SolacePrint.PrintType.Meta);
    }

    #endregion

    public static void Process(double delta)
    {
        SolacePrint.ApplyPrintQueue();
    }

    public static void FixedProcess(double delta)
    {
    }
}