using Godot;
using System;
public enum AttributeModType
{
    Flat,
    PercentAdd,
    PercentMult,
}

[GlobalClass]
public partial class AttributeModifier : Resource
{
    [Export] public float Value;
    [Export] public AttributeModType ModType;
    [Export] public int Order;
    public object Source;
}
