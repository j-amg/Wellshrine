using Godot;
using System;

public partial class EnemyChase : State
{
    public override void Enter()
    {
        owningEntity.audioPlayer.Stream = owningEntity.walkSound;
        owningEntity.audioPlayer.Play();
    }

    public override void Exit()
    {
        owningEntity.audioPlayer.Stop();
    }
    public override void Update(double delta)
    {
        Vector3 direction;
        Vector3 velocity = owningEntity.Velocity;
        owningEntity.nav.TargetPosition = Global.Singleton.player.GlobalPosition;
        direction = (owningEntity.nav.GetNextPathPosition() - owningEntity.GlobalPosition).Normalized();
        velocity = velocity.Lerp(direction * owningEntity.attributeData.Attributes[AttributeType.MovementSpeed].Value, (float)(owningEntity.acceleration * delta));
        owningEntity.Velocity = velocity;
        owningEntity.sprite.Play("run");
        if (owningEntity.stunned) owningEntity.sprite.Play("stun");
        if (owningEntity.dead) EmitSignal(SignalName.transition, "die");
        if (Global.Singleton.dead) EmitSignal(SignalName.transition, "idle");
        if ((Global.Singleton.player.GlobalPosition - owningEntity.GlobalPosition).Length() <= owningEntity.attackRange && !Global.Singleton.dead) EmitSignal(SignalName.transition, "attack");
    }
}
