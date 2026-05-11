using Godot;
using System;

public partial class HurtBox : Area3D
{
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area3D area)
	{

		if (area != null && area.Owner is Entity entity)
		{
			//Damage d = Damage.InitDamage(5f, false, null);
			//entity.TakeDamage(d);
		}
	}
}
