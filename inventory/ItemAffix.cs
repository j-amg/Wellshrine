using System;
using Godot;
public partial class ItemAffix : Resource
{
    [Export] public AttributeType TargetType;
    [Export] public AttributeModifier attributeModifier;
}
