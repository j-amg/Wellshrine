using Godot;
using System;
public enum AttributeModType
{
    Flat,
    PercentAdd,
    PercentMult,
}

public partial class AttributeModifier(float value, AttributeModType modType, object source) : Resource
{
    public float Value = value;
    public AttributeModType ModType = modType;
    public readonly object Source = source;
    public readonly int Order = (int)modType;
}
