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

	public void Initialise(DamageInst damageInst, Entity entity)
	{
		offset = new((float)GD.RandRange((double)-1, 1), (float)GD.RandRange(-1, (double)1));
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

	public override void _Process(double delta)
	{
		//base._PhysicsProcess(delta);
		Vector2 localCentre = new(Size.X / 2, Size.Y / 2);
		Position = GetViewport().GetCamera3D().UnprojectPosition(target.hitBox.GetNode<CollisionShape3D>("CollisionShape3D").GlobalPosition) - localCentre + offset.Normalized() * 100;

	}

	public async void Fade()
	{
		await ToSignal(GetTree().CreateTimer(.1), "timeout");
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate:a", 1, 0f).From(0f);
		tween.TweenProperty(this, "modulate:a", 0, 1f);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
