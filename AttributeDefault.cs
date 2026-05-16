using Godot;
using System;

[GlobalClass]

public partial class AttributeDefault(AttributeType _type, float _value, AttributeModType _modtype) : Resource
{
    [Export] public AttributeType type = _type;
    [Export] public float value = _value;
    [Export] public AttributeModType attributeModType = _modtype;
    public AttributeDefault() : this(0, 0, 0) { }
}
