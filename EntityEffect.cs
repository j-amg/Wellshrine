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
	[Export] public EntityEffectApplicationType applicationType = EntityEffectApplicationType.Stacking;
	[Export] public float effectInterval = 1;
	[Export] public float effectDuration = 5;
	public float effectStartTime;
	[Export] public Array<DamageData> damageDatas = [];
	public float lastAppliedTime;
	public Entity target;

	public virtual void ApplyEffect()
	{
		if (damageDatas.Count > 0)
		{
			DamagePackage damagePackage = new(damageDatas, false, this, target);
			target.TakeDamage(damagePackage);
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
