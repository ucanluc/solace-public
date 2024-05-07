using Godot;
using System;

[Tool]
public partial class NumeralHSlider : HSlider
{
    [Export] private string _prefixText = "";
    [Export] private string _suffixText = "";
    [Export] private Label? _textLabel;

    [Export] private bool _asPowerOfTwo;
    [Export] private bool _updateInEditor;

    public override void _Ready()
    {
        base._Ready();
        ValueChanged += UpdateLabel;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_updateInEditor && Engine.IsEditorHint())
        {
            UpdateLabel();
        }
    }

    private void UpdateLabel(double value)
    {
        if (_textLabel == null)
        {
            return;
        }

        _textLabel.Text = $"{_prefixText}{GetValue(value)}{_suffixText}";
    }

    private void UpdateLabel()
    {
        if (_textLabel == null)
        {
            return;
        }

        _textLabel.Text = $"{_prefixText}{GetValue()}{_suffixText}";
    }


    private int GetValue()
    {
        if (_asPowerOfTwo)
        {
            return (int)Mathf.Pow(2, Value);
        }

        return (int)Value;
    }

    private int GetValue(double value)
    {
        if (_asPowerOfTwo)
        {
            return (int)Mathf.Pow(2, value);
        }

        return (int)value;
    }
}