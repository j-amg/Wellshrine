using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class AttributeData : Resource
{
    public PlayerAttribute Strength;
    public PlayerAttribute Dexterity;
    public PlayerAttribute Intelligence;
    public Dictionary<AttributeType, PlayerAttribute> playerAttributes;

    public void InitialisePlayerAttributes()
    {
        playerAttributes.Add(AttributeType.Strength, Strength);
        playerAttributes.Add(AttributeType.Dexterity, Dexterity);
        playerAttributes.Add(AttributeType.Intelligence, Intelligence);
    }
}