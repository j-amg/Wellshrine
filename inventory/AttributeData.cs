using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class AttributeData : Resource
{
    [Export] float baseStrength = 50f;
    [Export] float baseDexterity = 50f;
    [Export] float baseIntelligence = 50f;
    [Export] float baseArmour = 0f;
    [Export] float baseHealth = 100f;
    [Export] float baseMana = 100f;
    [Export] float baseProjCount = 0f;
    [Export] float baseProjSpeed = 100f;
    [Export] float baseCastSpeed = 100f;

    public Dictionary<AttributeType, Attribute> Attributes = new()
        {
            {AttributeType.Strength, new Attribute(0)},
            {AttributeType.Dexterity, new Attribute(0)},
            {AttributeType.Intelligence, new Attribute(0)},
            {AttributeType.Armour, new Attribute(0)},
            {AttributeType.Health, new Attribute(0)},
            {AttributeType.Mana, new Attribute(0)},
            {AttributeType.ProjCount, new Attribute(0)},
            {AttributeType.ProjSpeed, new Attribute(0)},
            {AttributeType.CastSpeed, new Attribute(0)}
        };

    public void SetDefaultValues()
    {
        Attributes[AttributeType.Strength].SetBaseValue(baseStrength);
        Attributes[AttributeType.Dexterity].SetBaseValue(baseDexterity);
        Attributes[AttributeType.Intelligence].SetBaseValue(baseIntelligence);
        Attributes[AttributeType.Armour].SetBaseValue(baseArmour);
        Attributes[AttributeType.Health].SetBaseValue(baseHealth);
        Attributes[AttributeType.Mana].SetBaseValue(baseMana);
        Attributes[AttributeType.ProjCount].SetBaseValue(baseProjCount);
        Attributes[AttributeType.ProjSpeed].SetBaseValue(baseProjSpeed);
        Attributes[AttributeType.CastSpeed].SetBaseValue(baseCastSpeed);
    }

    public void UpdateBaseAttribute(AttributeType type, float value)
    {
        Attributes[type].SetBaseValue(value);
    }

}