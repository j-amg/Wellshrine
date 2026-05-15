using Godot;
using System;

public partial class EntityEffect : Resource
{

	public Entity target;

	public bool active = false;
	public float effectInterval;
	public float effectDuration;
	public float effectStartTime;

	public virtual void ApplyEffect(Entity entity)
	{
		return;
	}

	public virtual void AttachToEntity(Entity _target)
	{
		effectStartTime = Time.GetTicksMsec();
		target = _target;
	}

	public void OnEffectEnd()
	{
		GD.Print("effect end");
	}
}
