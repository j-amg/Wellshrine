using Godot;
using System;

public partial class ShooterAttack : State
{
    private Enemy enemy;
    [Export]
    public PackedScene projectile;
    private Vector3 velocity;

    public Projectile castProj;
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
        Projectile b = projectile.Instantiate() as Projectile;
        var main = GetTree().CurrentScene;
        b.Transform = enemy.Transform;
        b.LookAtFromPosition(b.Position + new Vector3(0,2.5f, 0), player.head.GlobalPosition);
        b.damage = damage;
        b.velocity = new Vector3(0,0,0);
        main.CallDeferred("add_child", b);
        castProj = b;
        await ToSignal(GetTree().CreateTimer(windup), "timeout");
        castProj = null;
        b?.LookAt(player.head.GlobalPosition);
        if (b != null) b.velocity = -b.Transform.Basis.Z * (b.muzzleVelocity + Global.Singleton.currentLevel/2);
        await ToSignal(GetTree().CreateTimer(duration), "timeout");
        EmitSignal(SignalName.transition, "chase");
    }
}
