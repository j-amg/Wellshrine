using Godot;
using System;

public partial class ShooterAttack : State
{
    [Export]
    public PackedScene projectile;
    public override void Enter()
    {
        owningEntity.Velocity = Vector3.Zero;
        Attack();
        owningEntity.sprite.Play("cast");
    }

    public async void Attack()
    {
        // Damage damage = Damage.InitDamage(owningEntity.damage, false, owningEntity);
        // Projectile b = projectile.Instantiate() as Projectile;
        // var main = GetTree().CurrentScene;
        // b.Transform = owningEntity.Transform;
        // b.LookAtFromPosition(b.Position + new Vector3(0,2.5f, 0), Global.Singleton.player.head.GlobalPosition);
        // b.damage = damage;
        // b.velocity = new Vector3(0,0,0);
        // b.owningEntity = owningEntity;
        // b.casting = true;
        // main.CallDeferred("add_child", b);
        // await ToSignal(GetTree().CreateTimer(owningEntity.attackWindup), "timeout");
        // if (b != null && !owningEntity.dead)
        // {
        //     Global.Singleton.PlaySound3D(owningEntity.GlobalPosition, owningEntity.attackSound);
        //     b.casting = false;
        //     b.LookAt(Global.Singleton.player.head.GlobalPosition);
        //     b.velocity = -b.Transform.Basis.Z * (b.muzzleVelocity + Global.Singleton.currentLevel/2);
        // } else b.Destroy();
        // await ToSignal(GetTree().CreateTimer(owningEntity.attackDuration), "timeout");
        // if (!owningEntity.dead || Global.Singleton.dead) EmitSignal(SignalName.transition, "chase");
    }

    public override void Update(double delta)
    {
        if (owningEntity.dead) EmitSignal(SignalName.transition, "die");
    }
}
