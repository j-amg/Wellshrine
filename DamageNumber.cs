using Godot;
using System;

public partial class DamageNumber : Label
{
	Vector2 offset;
	Entity target;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Fade();
	}

	public DamageNumber(DamageInst damageInst, Entity entity)
	{
		offset = new(GD.RandRange(0, 1), GD.RandRange(0, 1));
		target = entity;


		Text = ((int)damageInst.amount).ToString();

		switch (damageInst.type)
		{
			case DamageType.Physical:
				Modulate = new Color(1, 1, 1);
				break;
			case DamageType.Lightning:
				Modulate = new Color(1, 1, 0);
				break;
			case DamageType.Fire:
				Modulate = new Color(1, 0, 0);
				break;
			case DamageType.Cold:
				Modulate = new Color(0, 1, 0);
				break;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Position = GetViewport().GetCamera3D().UnprojectPosition(target.GlobalPosition) + offset.Normalized() * 50;

	}

	public void Fade()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate:a", 0, 1f);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
