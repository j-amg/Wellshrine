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

	public virtual void OnAttached(Entity _target)
	{
		effectStartTime = Time.GetTicksMsec();
		target = _target;
	}

    public override void _PhysicsProcess(double delta)
    {
		// timer apply effect to player on effect interval
        return;
    }
}
