using Godot;
using System;

public partial class ChaserAttack : State
{
    private Enemy enemy;
    private Vector3 velocity;
    public override void Enter()
    {
        base.Enter();
        enemy = GetOwner<Enemy>();
        velocity = Vector3.Zero;
        enemy.Velocity = velocity;
        Attack(enemy.attackWindup, enemy.attackDuration, enemy.attackDamage);
        enemy.sprite.Play("attack");
    }

    public async void Attack(float windup, float duration, float damage)
    {
        await ToSignal(GetTree().CreateTimer(windup), "timeout");
        GD.Print(Name + " attacks player for " + damage);
        if ((player.GlobalPosition - enemy.GlobalPosition).Length() <= enemy.attackRange)
        {
            if (player is IDamageable damageable) damageable.Damage(damage);
            GD.Print("hit");
        } else
        {
            GD.Print("miss");
        }
        await ToSignal(GetTree().CreateTimer(duration), "timeout");
        EmitSignal(SignalName.transition, "chase");
    }
}
