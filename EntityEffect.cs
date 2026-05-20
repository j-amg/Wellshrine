using Godot;
using System;
using Godot.Collections;

public enum EntityEffectApplicationType
{
	Stacking,
	Refreshing
}

[GlobalClass]
public partial class EntityEffect : Resource
{
	[Export] public string effectName = "def";
	[Export] public EntityEffectApplicationType applicationType = EntityEffectApplicationType.Stacking;
	[Export] public float effectInterval = 1;
	[Export] public float effectDuration = 5;
	public Entity sourceEntity;
	public float effectStartTime;
	[Export] public Array<DamageData> damageDatas = [];
	public float lastAppliedTime;
	public Entity target;

	public EntityEffect NewEffect(Entity _sourceEntity)
	{
		EntityEffect effect = (EntityEffect)Duplicate();
		effect.sourceEntity = _sourceEntity;
		return effect;

	}

	public virtual void ApplyEffect()
	{
		GD.Print(sourceEntity);
		if (damageDatas.Count > 0)
		{
			DamagePackage damagePackage = new(damageDatas, false, this, sourceEntity);
			damagePackage.Hit(target);
		}
	}

	public void Initialise(Entity entity)
	{
		target = entity;
		effectStartTime = Time.GetTicksMsec();
		lastAppliedTime = effectInterval * 1000;
	}

	public void Update()
	{
		float elapsedTimeMs = Time.GetTicksMsec() - effectStartTime;
		if (elapsedTimeMs >= lastAppliedTime)
		{
			lastAppliedTime += effectInterval * 1000;
			ApplyEffect();
		}

		if (elapsedTimeMs > effectDuration * 1000) target.entityEffects.Remove(this);
	}

	public void OnEffectEnd()
	{
		GD.Print("effect end");
	}
}
