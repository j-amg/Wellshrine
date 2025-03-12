using Godot;
using System;

public partial class ShooterAttack : State
{
    [Export]
    public PackedScene projectile;
    public override void Enter()
    {
        owner.Velocity = Vector3.Zero;
        Attack(owner.attackWindup, owner.attackDuration, owner.attackDamage);
        owner.sprite.Play("cast");
    }

    public async void Attack(float windup, float duration, float damage)
    {
        Projectile b = projectile.Instantiate() as Projectile;
        var main = GetTree().CurrentScene;
        b.Transform = owner.Transform;
        b.LookAtFromPosition(b.Position + new Vector3(0,2.5f, 0), Global.Singleton.player.head.GlobalPosition);
        b.damage = damage;
        b.velocity = new Vector3(0,0,0);
        await ToSignal(GetTree().CreateTimer(windup * .5), "timeout");
        main.CallDeferred("add_child", b);
        if (owner.dead && b != null) b.Destroy();
        await ToSignal(GetTree().CreateTimer(windup * .5), "timeout");
        if (owner.dead && b != null) b.Destroy(); else{
            b.LookAt(Global.Singleton.player.head.GlobalPosition);
            b.velocity = -b.Transform.Basis.Z * (b.muzzleVelocity + Global.Singleton.currentLevel/2);
        }
        await ToSignal(GetTree().CreateTimer(duration), "timeout");
        if (!owner.dead) EmitSignal(SignalName.transition, "chase");
    }

    public override void Update(double delta)
    {
        if (owner.dead) EmitSignal(SignalName.transition, "die");
    }
}
