using Godot;
using System;

public partial class ShooterAttack : State
{
    [Export]
    public PackedScene projectile;
    public override void Enter()
    {
        owner.Velocity = Vector3.Zero;
        Attack();
        owner.sprite.Play("cast");
    }

    public async void Attack()
    {
        Damage damage = Damage.InitDamage(owner.damage, false, owner);
        Projectile b = projectile.Instantiate() as Projectile;
        var main = GetTree().CurrentScene;
        b.Transform = owner.Transform;
        b.LookAtFromPosition(b.Position + new Vector3(0,2.5f, 0), Global.Singleton.player.head.GlobalPosition);
        b.damage = damage;
        b.velocity = new Vector3(0,0,0);
        await ToSignal(GetTree().CreateTimer(owner.attackWindup * .5), "timeout");
        Global.Singleton.PlaySound3D(owner.GlobalPosition, owner.attackSound);
        main.CallDeferred("add_child", b);
        if (owner.dead && b != null) b.Destroy();
        await ToSignal(GetTree().CreateTimer(owner.attackWindup * .5), "timeout");
        if (owner.dead && b != null) b.Destroy(); else{
            b.LookAt(Global.Singleton.player.head.GlobalPosition);
            b.velocity = -b.Transform.Basis.Z * (b.muzzleVelocity + Global.Singleton.currentLevel/2);
        }
        await ToSignal(GetTree().CreateTimer(owner.attackDuration), "timeout");
        if (!owner.dead || Global.Singleton.dead) EmitSignal(SignalName.transition, "chase");
    }

    public override void Update(double delta)
    {
        if (owner.dead) EmitSignal(SignalName.transition, "die");
    }
}
