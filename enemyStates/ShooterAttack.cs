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
        // Damage damage = Damage.InitDamage(owner.damage, false, owner);
        // Projectile b = projectile.Instantiate() as Projectile;
        // var main = GetTree().CurrentScene;
        // b.Transform = owner.Transform;
        // b.LookAtFromPosition(b.Position + new Vector3(0,2.5f, 0), Global.Singleton.player.head.GlobalPosition);
        // b.damage = damage;
        // b.velocity = new Vector3(0,0,0);
        // b.owner = owner;
        // b.casting = true;
        // main.CallDeferred("add_child", b);
        // await ToSignal(GetTree().CreateTimer(owner.attackWindup), "timeout");
        // if (b != null && !owner.dead)
        // {
        //     Global.Singleton.PlaySound3D(owner.GlobalPosition, owner.attackSound);
        //     b.casting = false;
        //     b.LookAt(Global.Singleton.player.head.GlobalPosition);
        //     b.velocity = -b.Transform.Basis.Z * (b.muzzleVelocity + Global.Singleton.currentLevel/2);
        // } else b.Destroy();
        // await ToSignal(GetTree().CreateTimer(owner.attackDuration), "timeout");
        // if (!owner.dead || Global.Singleton.dead) EmitSignal(SignalName.transition, "chase");
    }

    public override void Update(double delta)
    {
        if (owner.dead) EmitSignal(SignalName.transition, "die");
    }
}
