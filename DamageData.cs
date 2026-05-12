using Godot;
using System;

[GlobalClass]
public partial class DamageData : Resource
{
    [Export] public DamageType type;
    [Export] public float amountMin;
    [Export] public float amountMax;
}
