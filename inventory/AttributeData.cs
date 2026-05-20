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
    public System.Collections.Generic.Dictionary<AttributeType, Attribute> attributes = [];
    public void SetDefaultValues(Array<AttributeDefault> attributeOverrides)
    {
        attributes.Clear();
        foreach (var ad in Entity.attributeDefaults) attributes.Add(ad.type, new Attribute(ad.value, ad.attributeModType));
        foreach (var ad in attributeOverrides) attributes[ad.type].SetBaseValue(ad.value);
        EmitSignal(SignalName.ValuesSet);
    }
}