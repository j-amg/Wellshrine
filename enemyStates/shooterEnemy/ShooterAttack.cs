using Godot;
using System;

public partial class ShooterAttack : State
{
    private Enemy enemy;
    [Export]
    public PackedScene projectile;
    public override void Enter()
    {
        base.Enter();
        enemy = GetOwner<Enemy>();
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
		    main.CallDeferred("add_child", b);
		    b.Transform = enemy.GlobalTransform;
            b.LookAtFromPosition(b.Position + new Vector3(0,0,2.5f), player.GlobalPosition);
		    b.velocity = -b.Transform.Basis.Z * b.muzzleVelocity;
        }
        await ToSignal(GetTree().CreateTimer(duration), "timeout");
        EmitSignal(SignalName.transition, "chase");
    }
}
