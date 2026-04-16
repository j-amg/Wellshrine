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
		if (area != null && area.Owner is IDamageable damageable)
        {
            GD.Print("can hit");
            damageable.Damage(Damage.InitDamage(5f, false, GetParent()));
        } 
	}
}
