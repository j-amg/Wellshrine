using Godot;
using System;

public partial class ShooterAttack : State
{
    private Enemy enemy;
    [Export]
    public PackedScene projectile;
    private Vector3 velocity;
    public override void Enter()
    {
        base.Enter();
        enemy = GetOwner<Enemy>();
        velocity = Vector3.Zero;
        enemy.Velocity = velocity;
        Attack(enemy.attackWindup, enemy.attackDuration, enemy.attackDamage);
    }

    public async void Attack(float windup, float duration, float damage)
    {
        await ToSignal(GetTree().CreateTimer(windup), "timeout");
        GD.Print(Name + " shoots at player for " + damage);
        if ((player.GlobalPosition - enemy.GlobalPosition).Length() <= enemy.attackRange)
        { 
            Projectile b = projectile.Instantiate() as Projectile;
            var main = GetTree().CurrentScene;
            b.Transform = enemy.Transform;
            b.LookAtFromPosition(b.Position + new Vector3(0,2.5f, 0), player.head.GlobalPosition);
		    b.velocity = -b.Transform.Basis.Z * b.muzzleVelocity;
            b.damage = damage;
            main.CallDeferred("add_child", b);
        }
        await ToSignal(GetTree().CreateTimer(duration), "timeout");
        EmitSignal(SignalName.transition, "chase");
    }
}
