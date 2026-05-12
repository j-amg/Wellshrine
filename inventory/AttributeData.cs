using System;
using System.Collections.Generic;
using Godot;

public enum AttributeType
{
    Strength,
    Dexterity,
    Intelligence,
    Armour,
    MaxHealth,
    MaxMana,
    HealthRegen,
    TotalDamage,
    PhysicalDamage,
    LightningDamage,
    FireDamage,
    ColdDamage,
    LightningResist,
    FireResist,
    ColdResist,
    ProjCount,
    CastSpeed,
    ProjSpeed,
    MovSpeed
}

[GlobalClass]

public partial class AttributeData : Resource
{
    public static Dictionary<AttributeType, string> AttributeDisplayNames = new()
	{
		{AttributeType.Strength, "Strength"},
		{AttributeType.Dexterity, "Dexterity"},
		{AttributeType.Intelligence, "Intelligence"},
		{AttributeType.Armour, "Armour"},
        {AttributeType.MaxHealth, "Health"},
        {AttributeType.MaxMana, "Mana"},
        {AttributeType.TotalDamage, "Total Damage"},
        {AttributeType.PhysicalDamage, "Physical Damage"},
        {AttributeType.LightningDamage, "Lighning Damage"},
        {AttributeType.FireDamage, "Fire Damage"},
        {AttributeType.ColdDamage, "Cold Damage"},
        {AttributeType.LightningResist, "Lightning Resistance"},
        {AttributeType.FireResist, "Fire Resistance"},
        {AttributeType.ColdResist, "Cold Resistance"},
		{AttributeType.HealthRegen, "Health Regeneration"},
		{AttributeType.ProjCount, "Projectile Count"},
		{AttributeType.CastSpeed, "Cast Speed"},
		{AttributeType.ProjSpeed, "Projectile Speed"},
        {AttributeType.MovSpeed, "Movement Speed"}

	};
    [Signal] public delegate void DefaultValuesSetEventHandler();
    [Export] float baseStrength = 50f;
    [Export] float baseDexterity = 50f;
    [Export] float baseIntelligence = 50f;
    [Export] float baseArmour = 0f;
    [Export] float baseMaxHealth = 100f;
    [Export] float baseMaxMana = 100f;
    [Export] float baseHealthRegen = 0f;
    [Export] float baseTotalDamage = 100f;
    [Export] float basePhysicalDamage = 0f;
    [Export] float baseLightningDamage = 0f;
    [Export] float baseFireDamage = 0f;
    [Export] float baseColdDamage = 0f;
    [Export] float baseLightningResist = 0f;
    [Export] float baseFireResist = 0f;
    [Export] float baseColdResist = 0f;
    [Export] float baseProjCount = 0f;
    [Export] float baseProjSpeed = 100f;
    [Export] float baseCastSpeed = 100f;
    [Export] float baseMovSpeed = 0f;

    public Dictionary<AttributeType, Attribute> Attributes = new()
        {
            {AttributeType.Strength, new Attribute(0)},
            {AttributeType.Dexterity, new Attribute(0)},
            {AttributeType.Intelligence, new Attribute(0)},
            {AttributeType.Armour, new Attribute(0)},
            {AttributeType.MaxHealth, new Attribute(0)},
            {AttributeType.MaxMana, new Attribute(0)},
            {AttributeType.TotalDamage, new Attribute(0)},
            {AttributeType.PhysicalDamage, new Attribute(0)},
            {AttributeType.LightningDamage, new Attribute(0)},
            {AttributeType.FireDamage, new Attribute(0)},
            {AttributeType.ColdDamage, new Attribute(0)},
            {AttributeType.LightningResist, new Attribute(0)},
            {AttributeType.FireResist, new Attribute(0)},
            {AttributeType.ColdResist, new Attribute(0)},
            {AttributeType.ProjCount, new Attribute(0)},
            {AttributeType.ProjSpeed, new Attribute(0)},
            {AttributeType.CastSpeed, new Attribute(0)},
            {AttributeType.MovSpeed, new Attribute(0)}
        };

    public void SetDefaultValues()
    {
        Attributes[AttributeType.Strength].SetBaseValue(baseStrength);
        Attributes[AttributeType.Dexterity].SetBaseValue(baseDexterity);
        Attributes[AttributeType.Intelligence].SetBaseValue(baseIntelligence);
        Attributes[AttributeType.Armour].SetBaseValue(baseArmour);
        Attributes[AttributeType.MaxHealth].SetBaseValue(baseMaxHealth);
        Attributes[AttributeType.MaxMana].SetBaseValue(baseMaxMana);
        Attributes[AttributeType.TotalDamage].SetBaseValue(baseTotalDamage);
        Attributes[AttributeType.PhysicalDamage].SetBaseValue(basePhysicalDamage);
        Attributes[AttributeType.LightningDamage].SetBaseValue(baseLightningDamage);
        Attributes[AttributeType.FireDamage].SetBaseValue(baseFireDamage);
        Attributes[AttributeType.ColdDamage].SetBaseValue(baseColdDamage);
        Attributes[AttributeType.LightningResist].SetBaseValue(baseLightningResist);
        Attributes[AttributeType.FireResist].SetBaseValue(baseFireResist);
        Attributes[AttributeType.ColdResist].SetBaseValue(baseColdResist);
        Attributes[AttributeType.ProjCount].SetBaseValue(baseProjCount);
        Attributes[AttributeType.ProjSpeed].SetBaseValue(baseProjSpeed);
        Attributes[AttributeType.CastSpeed].SetBaseValue(baseCastSpeed);
        EmitSignal(SignalName.DefaultValuesSet);
    }

    public void UpdateBaseAttribute(AttributeType type, float value)
    {
        Attributes[type].SetBaseValue(value);
    }
}