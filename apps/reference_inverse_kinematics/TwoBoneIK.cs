using Godot;
using System;

[Tool]
public partial class TwoBoneIK : Node3D
{
    [Export] private Node3D? _bone1;
    [Export] private Node3D? _bone2;
    [Export] private Node3D? _endPoint;
    [Export] private Node3D? _target;
    
    
}