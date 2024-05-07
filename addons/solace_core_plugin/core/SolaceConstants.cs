namespace Solace.addons.solace_core_plugin.core;

/// <summary>
/// Assumed unit constants for asset / generation compatibility. 
/// </summary>
public static class SolaceConstants
{
    /// <summary>
    /// Conversion between 3d and 2d objects are done to 3.125 cm of precision. 
    /// </summary>
    public const int GodotPixelsPerMeter = 32;
    
    /// <summary>
    /// Topdown building layout is calculated over 12.5 cm x 12.5 cm squares.
    /// </summary>
    public const int BlueprintSubdivisionPerMeter = 8;

    /// <summary>
    /// Human characters occupy 0.5 x 0.5 m space minimum when standing.
    /// </summary>
    public const int CharacterMinBlueprintWidth = 4;

    /// <summary>
    /// Human characters navigate over 1 x 1 m tiles, which includes the nearest walls.
    /// </summary>
    public const int CharacterNavBlueprintWidth = 8;
}