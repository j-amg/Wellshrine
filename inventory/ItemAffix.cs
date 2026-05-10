using System;
using Godot;

[GlobalClass]
public partial class ItemAffix : Resource
{
    [Export] public string Name;
    [Export] public AttributeType TargetType;
    [Export] public AttributeModifier attributeModifier;
}
