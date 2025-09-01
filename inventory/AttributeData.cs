using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class AttributeData : Resource
{
    public Dictionary<AttributeType, PlayerAttribute> playerAttributes = [];

    public void InitialisePlayerAttributes()
    {
        foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
        {
            playerAttributes.Add(type, new PlayerAttribute(0.0f));
        }
    }
}