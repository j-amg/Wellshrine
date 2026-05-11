using Godot;
using System;
using Godot.Collections;

public enum DamageType
{
    Physical,
    Lightning,
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
        return d;
    }

    public void Hit() => EmitSignal(SignalName.damageExecuted, this);
}

[GlobalClass]
public partial class DamageInst : Resource
{
    public DamageType type;
    public float amount;
    public DamageInst(float amountMin, float amountMax, DamageType _type, Entity entity = null)
    {

        amount = (float)GD.RandRange(amountMin, amountMax);
        type = _type;
        if (entity != null) amount = ScaleAmountToEntity(amount, type, entity);
    }

    public float ScaleAmountToEntity(float amount, DamageType type, Entity entity)
    {
        float scaledAmount = amount;

        switch (type)
        {
            case DamageType.Physical:
                scaledAmount *= entity.attributeData.Attributes[AttributeType.PhysicalDamage].Value / 100;
                break;
            case DamageType.Lightning:
                scaledAmount *= entity.attributeData.Attributes[AttributeType.LightningDamage].Value / 100;
                break;
            case DamageType.Fire:
                scaledAmount *= entity.attributeData.Attributes[AttributeType.FireDamage].Value / 100;
                break;
            case DamageType.Cold:
                scaledAmount *= entity.attributeData.Attributes[AttributeType.ColdDamage].Value / 100;
                break;
        }
        scaledAmount *= entity.attributeData.Attributes[AttributeType.TotalDamage].Value / 100;

        return scaledAmount;
    }
}