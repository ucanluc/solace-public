using System;
using System.Collections.Generic;
using Godot;

namespace Solace.addons.solace_core_plugin.lib.generator.wfc;

/// <summary>
/// Defines a volume in 3d space.
/// </summary>
public struct WfcCell
{
    private Vector3I coordinates;
    private WfcOption[] assignedOptions;
}