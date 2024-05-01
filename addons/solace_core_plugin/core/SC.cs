namespace Solace.addons.solace_core_plugin.core;

/// <summary>
/// Solace Core's global functions
/// </summary>

// ReSharper disable once InconsistentNaming
public static class SC
{
    #region Printing

    public static void PrintErr(string name, string text) =>
        SolacePrint.Print($"{name}: {text}", SolacePrint.PrintType.Error);
    public static void PrintWarn(string name, string text) =>
        SolacePrint.Print($"{name}: {text}", SolacePrint.PrintType.Warning);
    public static void PrintVerbose(string name, string text) =>
        SolacePrint.Print($"{name}: {text}", SolacePrint.PrintType.Verbose);
    public static void Print(string name, string text) =>
        SolacePrint.Print($"{name}: {text}", SolacePrint.PrintType.NamedObject);

    public static void Print(string text, SolacePrint.PrintType printType) =>
        SolacePrint.Print($"*: {text}", printType);

    #endregion
}