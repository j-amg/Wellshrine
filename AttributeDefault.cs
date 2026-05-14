using Godot;
using System;

[GlobalClass]

public partial class AttributeDefault(AttributeType _type, float _value) : Resource
{
    [Export] public AttributeType type = _type;
    [Export] public float value = _value;
    public AttributeDefault() : this(0, 0) { }
}
