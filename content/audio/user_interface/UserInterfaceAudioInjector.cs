using Godot;
using Solace.addons.solace_core_plugin.core;
using Solace.addons.solace_core_plugin.core.injection;

namespace Solace.content.audio.user_interface;

public partial class UserInterfaceAudioInjector : Node
{
    [Export] private AudioStreamPlayer? _clickSound;


    public override void _EnterTree()
    {
        base._EnterTree();

        // TODO; A custom 'UI Audio Injections' or some other localised injection method will be needed.
        var success = SignalInjection.RegisterHook(
            BaseButton.SignalName.Pressed,
            new Callable(this, MethodName.PlayClickSound));
        if (!success)
        {
            SC.PrintWarn(nameof(UserInterfaceAudioInjector), "Button audio injection was ignored.");
        }
    }

    private void PlayClickSound()
    {
        _clickSound?.Play();
    }
}