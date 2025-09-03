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
    // public readonly int Order;

    // public AttributeModifier(float value, AttributeModType modType, object source)
    // {
    //     Value = value;
    //     ModType = modType;
    //     Source = source;
    //     Order = (int)modType;
    // }
}
