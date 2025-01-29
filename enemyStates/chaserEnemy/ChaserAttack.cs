using Godot;
using System;

public partial class ChaserAttack : State
{
    private Enemy enemy;

    public override void Enter()
    {
        base.Enter();
        enemy = GetOwner<Enemy>();
        Attack(enemy.attackWindup, enemy.attackDuration, enemy.attackDamage);
    }

    public async void Attack(float windup, float duration, float damage)
    {
        await ToSignal(GetTree().CreateTimer(windup), "timeout");
        GD.Print(Name + " attacks player for " + damage);
        if ((player.GlobalPosition - enemy.GlobalPosition).Length() <= enemy.attackRange)
        {
            GD.Print("hit");
        } else
        {
            GD.Print("miss");
        }
        await ToSignal(GetTree().CreateTimer(duration), "timeout");
        EmitSignal(SignalName.transition, "chase");
    }
}
