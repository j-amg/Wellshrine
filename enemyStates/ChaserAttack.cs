using Godot;
using System;

public partial class ChaserAttack : State
{
	public override void Enter()
	{
		owningEntity.Velocity = Vector3.Zero;
		Attack(owningEntity.attackWindup, owningEntity.attackDuration);
		owningEntity.sprite.Play("attack");
	}

	public async void Attack(float windup, float duration)
	{
		Damage damage = Damage.InitDamage(owningEntity.damage, false, owningEntity);
		await ToSignal(GetTree().CreateTimer(windup), "timeout");
		Global.Singleton.PlaySound3D(owningEntity.GlobalPosition, owningEntity.attackSound);
		GD.Print(Name + " attacks player for " + damage);
		if ((Global.Singleton.player.GlobalPosition - owningEntity.GlobalPosition).Length() <= owningEntity.attackRange)
		{
			((IDamageable)Global.Singleton.player).Damage(damage);
		}
		await ToSignal(GetTree().CreateTimer(duration), "timeout");
		 if (!owningEntity.dead  || Global.Singleton.dead) EmitSignal(SignalName.transition, "chase");
	}

	public override void Update(double delta)
	{
		if (owningEntity.dead) EmitSignal(SignalName.transition, "die");
	}

}
