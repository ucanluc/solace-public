using System;
using Godot;

namespace Solace.apps.stub.reference_knowledge;

public partial class LevelGenerator : Node
{
    [Export] private bool _pauseAfterStep = false;
    [Export] private bool _doGenStep = false;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        CheckGeneratorStep();
    }

    private void CheckGeneratorStep()
    {
        if (!_doGenStep)
        {
            return;
        }

        if (_pauseAfterStep)
        {
            _doGenStep = false;
        }

        DoGeneratorStep();
    }

    private void DoGeneratorStep()
    {
        throw new NotImplementedException();
    }
}