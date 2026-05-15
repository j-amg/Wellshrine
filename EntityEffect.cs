using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class EntityEffect : Resource
{

	[Export] public float effectInterval = 1;
	[Export] public float effectDuration = 100;
	public float effectStartTime;
	[Export] public Array<DamageData> damageDatas = [];
	
	public virtual void ApplyEffect(Entity entity)
	{
		GD.Print("apply effect");
		if (damageDatas.Count > 0)
		{
			DamagePackage damagePackage = new(damageDatas, false, this, entity);
			entity.TakeDamage(damagePackage);
		}
	}

	public void OnEffectEnd()
	{
		GD.Print("effect end");
	}
}
