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
        enemy.sprite.Play("cast");
    }

    public async void Attack(float windup, float duration, float damage)
    {
        Projectile b = projectile.Instantiate() as Projectile;
        var main = GetTree().CurrentScene;
        b.Transform = enemy.Transform;
        b.LookAtFromPosition(b.Position + new Vector3(0,2.5f, 0), player.head.GlobalPosition);
        b.damage = damage;
        b.velocity = new Vector3(0,0,0);
        await ToSignal(GetTree().CreateTimer(windup * .25), "timeout");
        main.CallDeferred("add_child", b);
        await ToSignal(GetTree().CreateTimer(windup * .75), "timeout");
        if (enemy.dead && b != null) b.Destroy(); else{
            b.LookAt(player.head.GlobalPosition);
            b.velocity = -b.Transform.Basis.Z * (b.muzzleVelocity + Global.Singleton.currentLevel/2);
        }
        await ToSignal(GetTree().CreateTimer(duration), "timeout");
        if (!enemy.dead) EmitSignal(SignalName.transition, "chase");
    }

    public override void Update(double delta)
    {
        if (enemy.dead) EmitSignal(SignalName.transition, "die");
    }
}
