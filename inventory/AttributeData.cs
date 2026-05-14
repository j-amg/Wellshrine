using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
public enum AttributeType
{
    Strength,
    Dexterity,
    Intelligence,
    Armour,
    MaximumHealth,
    MaximumMana,
    HealthRegen,
    TotalDamage,
    PhysicalDamage,
    LightningDamage,
    FireDamage,
    ColdDamage,
    FlatLightningDamage,
    FlatFireDamage,
    FlatColdDamage,
    LightningResistance,
    FireResistance,
    ColdResistance,
    ProjectileCount,
    CastSpeed,
    ProjectileSpeed,
    MovementSpeed
}

[GlobalClass]
public partial class AttributeData : Resource
{
    [Signal] public delegate void ValuesSetEventHandler();
    private Array<AttributeDefault> attributeDefaults = [
        new AttributeDefault(AttributeType.Strength, 50),
        new AttributeDefault(AttributeType.Dexterity, 50),
        new AttributeDefault(AttributeType.Intelligence, 50),
        new AttributeDefault(AttributeType.Armour, 0),
        new AttributeDefault(AttributeType.MaximumHealth, 100),
        new AttributeDefault(AttributeType.MaximumMana, 100),
        new AttributeDefault(AttributeType.TotalDamage, 100),
        new AttributeDefault(AttributeType.PhysicalDamage, 100),
        new AttributeDefault(AttributeType.LightningDamage, 100),
        new AttributeDefault(AttributeType.FireDamage, 100),
        new AttributeDefault(AttributeType.ColdDamage, 100),
        new AttributeDefault(AttributeType.FlatLightningDamage, 0),
        new AttributeDefault(AttributeType.FlatFireDamage, 0),
        new AttributeDefault(AttributeType.FlatColdDamage, 0),
        new AttributeDefault(AttributeType.LightningResistance, 0),
        new AttributeDefault(AttributeType.FireResistance, 0),
        new AttributeDefault(AttributeType.ColdResistance, 0),
        new AttributeDefault(AttributeType.HealthRegen, 0),
        new AttributeDefault(AttributeType.ProjectileCount, 0),
        new AttributeDefault(AttributeType.CastSpeed, 100),
        new AttributeDefault(AttributeType.ProjectileSpeed, 100),
        new AttributeDefault(AttributeType.MovementSpeed, 100),
    ];
    public System.Collections.Generic.Dictionary<AttributeType, Attribute> attributes = [];
    public void SetDefaultValues(Array<AttributeDefault> attributeOverrides)
    {
        attributes.Clear();
        foreach (var ad in attributeDefaults) attributes.Add(ad.type, new Attribute(ad.value));
        foreach (var ad in attributeOverrides) attributes[ad.type].SetBaseValue(ad.value);
        EmitSignal(SignalName.ValuesSet);
    }
}