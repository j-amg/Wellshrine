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
    public Array<DamageInst> damageInstances = [];
    public bool crit;
    public dynamic source;
    public Entity sourceEntity;

    public DamagePackage(Array<DamageData> damageDatas, bool _crit, dynamic _source, Entity _sourceEntity = null)
    {
        foreach (DamageData damageData in damageDatas)
        {
            damageInstances.Add(new DamageInst(damageData, _sourceEntity));
        }
        crit = _crit;
        source = _source;
        sourceEntity = _sourceEntity;

        if (sourceEntity == null) return;

        float lightningDamageVal = sourceEntity.attributeData.attributes[AttributeType.FlatLightningDamage].Value;
        if (lightningDamageVal != 0) damageInstances.Add(new DamageInst(new DamageData(DamageType.Lightning, lightningDamageVal, lightningDamageVal), sourceEntity));
        float fireDamageVal = sourceEntity.attributeData.attributes[AttributeType.FlatFireDamage].Value;
        if (fireDamageVal != 0) damageInstances.Add(new DamageInst(new DamageData(DamageType.Fire, fireDamageVal, fireDamageVal), sourceEntity));
        float coldDamageVal = sourceEntity.attributeData.attributes[AttributeType.FlatColdDamage].Value;
        if (coldDamageVal != 0) damageInstances.Add(new DamageInst(new DamageData(DamageType.Cold, coldDamageVal, coldDamageVal), sourceEntity));
    }

    public void Hit(Entity entity)
    {
        entity.EmitSignal(Entity.SignalName.DamageTaken, entity, this, sourceEntity);
        sourceEntity.EmitSignal(Entity.SignalName.DamageExecuted, sourceEntity, this, entity);
        foreach (DamageInst damage in damageInstances)
        {
            DamageInst scaledInst = DamageInst.ScaleToEntityDefense(damage, entity);
            entity.IncrementHealth(-scaledInst.amount);
        }
    }
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
                scaledAmount *= 1 + entity.attributeData.attributes[AttributeType.PhysicalDamage].Value / 100;
                break;
            case DamageType.Lightning:
                scaledAmount *= 1 + entity.attributeData.attributes[AttributeType.LightningDamage].Value / 100;
                break;
            case DamageType.Fire:
                scaledAmount *= 1 + entity.attributeData.attributes[AttributeType.FireDamage].Value / 100;
                break;
            case DamageType.Cold:
                scaledAmount *= 1 + entity.attributeData.attributes[AttributeType.ColdDamage].Value / 100;
                break;
        }
        scaledAmount *= 1 + entity.attributeData.attributes[AttributeType.TotalDamage].Value / 100;

        damageInst.amount = scaledAmount;

        return damageInst;
    }

    public static DamageInst ScaleToEntityDefense(DamageInst damageInst, Entity entity)
    {
        float scaledAmount = damageInst.amount;
        switch (damageInst.type)
        {
            // case DamageType.Physical:
            //     scaledAmount *= 1 / (entity.attributeData.attributes[AttributeType.Armour].Value / 100);
            //     break;
            case DamageType.Lightning:
                scaledAmount *= 1 - (entity.attributeData.attributes[AttributeType.LightningResistance].Value / 100);
                break;
            case DamageType.Fire:
                scaledAmount *= 1 - (entity.attributeData.attributes[AttributeType.FireResistance].Value / 100);
                break;
            case DamageType.Cold:
                scaledAmount *= 1 - (entity.attributeData.attributes[AttributeType.ColdResistance].Value / 100);
                break;
        }
        damageInst.amount = scaledAmount;
        return damageInst;
    }
}