using Godot;
using System;

[GlobalClass]
public partial class DamageData(DamageType _type, float _min, float _max) : Resource
{
    [Export] public DamageType type = _type;
    [Export] public float amountMin = _min;
    [Export] public float amountMax = _max;
    public DamageData() : this(0, 0, 0) { }
}
