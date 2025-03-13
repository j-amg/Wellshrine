using Godot;
using System;

public partial class EnemyChase : State
{
    public override void Enter()
    {
        base.Enter();
        owner.audioPlayer.Stream = owner.walkSound;
        owner.audioPlayer.Play();
    }

    public override void Exit()
    {
        base.Enter();
        owner.audioPlayer.Stop();
    }
    public override void Update(double delta)
    {
        Enemy enemy = Owner as Enemy;
        Vector3 direction;
		Vector3 velocity = enemy.Velocity;
		enemy.nav.TargetPosition = Global.Singleton.player.GlobalPosition;
		direction = (enemy.nav.GetNextPathPosition() - enemy.GlobalPosition).Normalized();
		velocity = velocity.Lerp(direction * enemy.baseMovementSpeed, (float)(enemy.acceleration * delta));
		enemy.Velocity = velocity;
        enemy.sprite.Play("run");
        if (enemy.stunned) enemy.sprite.Play("stun");
        if (enemy.dead) EmitSignal(SignalName.transition, "die");
        if (Global.Singleton.dead) EmitSignal(SignalName.transition, "idle");
        if ((Global.Singleton.player.GlobalPosition - enemy.GlobalPosition).Length() <= enemy.attackRange) EmitSignal(SignalName.transition, "attack");
    }
}
