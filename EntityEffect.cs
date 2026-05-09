using Godot;
using System;

public partial class EntityEffect : Node
{

	public Entity target;

	public bool active = false;
	public float effectInterval;
	public float effectDuration;
	public float effectStartTime;

	public override void _Ready()
	{
		return;
	}

	public virtual void AttachToEntity(Entity _target)
	{
		effectStartTime = Time.GetTicksMsec();
		target = _target;
	}

	public override void _PhysicsProcess(double delta)
	{
		float elapsedTimeSecs = Time.GetTicksMsec() - effectStartTime / 1000;

		if (elapsedTimeSecs % effectInterval == 0)
		{
			// apply effect
		}

		if (elapsedTimeSecs > effectDuration)
		{
			OnEffectEnd();
		}
		// timer apply effect to player on effect interval
		return;
	}

	private void EndEffect()
	{
		QueueFree();
	}

	private void OnEffectEnd()
	{
		GD.Print("effect end");
	}
}
