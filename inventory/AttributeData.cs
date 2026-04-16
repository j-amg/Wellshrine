using System;
using System.Collections.Generic;
using Godot;

public enum AttributeType
{
    Strength,
    Dexterity,
    Intelligence,
    Armour,
    ProjCount
}

[GlobalClass]
public partial class AttributeData : Resource
{
    public Dictionary<AttributeType, PlayerAttribute> playerAttributes = new()
    {
        {AttributeType.Strength, new PlayerAttribute(50f)},
        {AttributeType.Dexterity, new PlayerAttribute(50f)},
        {AttributeType.Intelligence, new PlayerAttribute(50f)},
        {AttributeType.Armour, new PlayerAttribute(0f)},
        {AttributeType.ProjCount, new PlayerAttribute(0f)}
    };
}