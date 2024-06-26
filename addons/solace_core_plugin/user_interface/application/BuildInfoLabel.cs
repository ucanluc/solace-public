﻿using Godot;

namespace Solace.addons.solace_core_plugin.user_interface.application;

[Tool]
public partial class BuildInfoLabel : Label
{
    private const string VersionNamePath = "application/config/version";

    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        UpdateLabel();
    }


    private void UpdateLabel()
    {
        var version = ProjectSettings.GetSetting(VersionNamePath).ToString();
        Text = $"Build {version}";
    }
}