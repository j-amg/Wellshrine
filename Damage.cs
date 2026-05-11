using Godot;
using System;
using Godot.Collections;

public enum DamageType
{
    Physical,
    Lighting,
    Fire,
    Cold
}


[GlobalClass]
public partial class DamagePackage : Resource
{
    [Signal]
    public delegate void damageExecutedEventHandler(DamagePackage damagePackage);
    public Array<DamageInst> damageInstances = [];
    public bool crit;
    public dynamic source;
    public Entity sourceEntity;

    public static DamagePackage InitDamage(Array<DamageInst> _damageInstances, bool _crit, dynamic _source, Entity _sourceEntity = null)
    {
        DamagePackage d = new() { damageInstances = _damageInstances, crit = _crit, source = _source, sourceEntity = _sourceEntity };
        if (_sourceEntity != null) d = ApplyEntityModifiers(d);
        return d;
    }

    private static DamagePackage ApplyEntityModifiers(DamagePackage damagePackage)
    {
        foreach (DamageInst di in damagePackage.damageInstances)
        {
            di.amount *= damagePackage.sourceEntity.attributeData.Attributes[AttributeType.TotalDamage].Value / 100;
        }
        return damagePackage;
    }

    public void Hit() => EmitSignal(SignalName.damageExecuted, this);
}

[GlobalClass]
public partial class DamageInst : Resource
{
    [Export] public DamageType type;
    [Export] public float baseAmountMin;
    [Export] public float baseAmountMax;
    public float amount;

    public void SetAmmount()
    {
        amount = (float)GD.RandRange(baseAmountMin, baseAmountMax);
    }
}