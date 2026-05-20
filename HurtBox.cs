using Godot;
using System;

public partial class HurtBox : Area3D
{
	public DamagePackage damagePackage;
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area3D area)
	{
		if (area != null && area.Owner is Entity entity)
		{
			damagePackage.Hit(entity);
		}
	}
}
