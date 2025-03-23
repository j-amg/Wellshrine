using Godot;
using System;

public partial class ChaserAttack : State
{
    public override void Enter()
    {
        owner.Velocity = Vector3.Zero;
        Attack(owner.attackWindup, owner.attackDuration);
        owner.sprite.Play("attack");
    }

    public async void Attack(float windup, float duration)
    {
        Damage damage = Damage.InitDamage(owner.damage, false, owner);
        await ToSignal(GetTree().CreateTimer(windup), "timeout");
        GD.Print(Name + " attacks player for " + damage);
        if ((Global.Singleton.player.GlobalPosition - owner.GlobalPosition).Length() <= owner.attackRange)
        {
            ((IDamageable)Global.Singleton.player).Damage(damage);
        }
        await ToSignal(GetTree().CreateTimer(duration), "timeout");
         if (!owner.dead  || Global.Singleton.dead) EmitSignal(SignalName.transition, "chase");
    }

    public override void Update(double delta)
    {
        if (owner.dead) EmitSignal(SignalName.transition, "die");
    }

}
