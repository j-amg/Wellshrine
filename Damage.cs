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

    public DamagePackage(Array<DamageInst> _damageInstances, bool _crit, dynamic _source, Entity _sourceEntity = null)
    {
        damageInstances = _damageInstances;
        crit = _crit;
        source = _source;
        sourceEntity = _sourceEntity;
    }

    public void Hit() => EmitSignal(SignalName.damageExecuted, this);
}

[GlobalClass]
public partial class DamageInst : Resource
{
    public DamageType type;
    public float amount;
    public DamageInst(DamageData damageData, Entity entity = null)
    {
        amount = (float)GD.RandRange(damageData.amountMin, damageData.amountMax);
        type = damageData.type;
        if (entity != null) amount = ScaleToEntityAttack(this, entity).amount;
    }

    public static DamageInst ScaleToEntityAttack(DamageInst damageInst, Entity entity)
    {
        float scaledAmount = damageInst.amount;

        switch (damageInst.type)
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

        damageInst.amount = scaledAmount;

        return damageInst;
    }

    public static DamageInst ScaleToEntityDefense(DamageInst damageInst, Entity entity)
    {
        float scaledAmount = damageInst.amount;

        switch (damageInst.type)
        {
            // case DamageType.Physical:
            //     scaledAmount *= 1 / (entity.attributeData.Attributes[AttributeType.Armour].Value / 100);
            //     break;
            case DamageType.Lightning:
                scaledAmount *= 1 / (entity.attributeData.Attributes[AttributeType.LightningResist].Value / 100);
                break;
            case DamageType.Fire:
                scaledAmount *= 1 / (entity.attributeData.Attributes[AttributeType.FireResist].Value / 100);
                break;
            case DamageType.Cold:
                scaledAmount *= 1 / (entity.attributeData.Attributes[AttributeType.ColdResist].Value / 100);
                break;
        }
        damageInst.amount = scaledAmount;
        return damageInst;
    }
}