using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class AttributeData : Resource
{
    public Dictionary<AttributeType, PlayerAttribute> playerAttributes = new Dictionary<AttributeType, PlayerAttribute>
    {
        {AttributeType.Strength, new PlayerAttribute(50f)},
        {AttributeType.Dexterity, new PlayerAttribute(50f)},
        {AttributeType.Intelligence, new PlayerAttribute(50f)}
    };
}