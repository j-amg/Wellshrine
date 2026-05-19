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
        new AttributeDefault(AttributeType.Strength, 50, AttributeModType.Flat),
        new AttributeDefault(AttributeType.Dexterity, 50, AttributeModType.Flat),
        new AttributeDefault(AttributeType.Intelligence, 50, AttributeModType.Flat),
        new AttributeDefault(AttributeType.Armour, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.MaximumHealth, 100, AttributeModType.Flat),
        new AttributeDefault(AttributeType.MaximumMana, 100, AttributeModType.Flat),
        new AttributeDefault(AttributeType.TotalDamage, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.PhysicalDamage, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.LightningDamage, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.FireDamage, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.ColdDamage, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.FlatLightningDamage, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.FlatFireDamage, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.FlatColdDamage, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.LightningResistance, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.FireResistance, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.ColdResistance, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.HealthRegen, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.ProjectileCount, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.CastSpeed, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.ProjectileSpeed, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.MovementSpeed, 0, AttributeModType.PercentAdd),
    ];
    public System.Collections.Generic.Dictionary<AttributeType, Attribute> attributes = [];
    public void SetDefaultValues(Array<AttributeDefault> attributeOverrides)
    {
        attributes.Clear();
        foreach (var ad in attributeDefaults) 
        {
            attributes.Add(ad.type, new Attribute(ad.value, ad.attributeModType));
        }
        foreach (var ad in attributeOverrides) attributes[ad.type].SetBaseValue(ad.value);
        EmitSignal(SignalName.ValuesSet);
    }
}