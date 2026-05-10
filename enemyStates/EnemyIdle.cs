using Godot;
using System;

public partial class EnemyIdle : State
{
    public override void Update(double delta)
    {
        if (owningEntity.awake) owningEntity.sprite.Play("idle"); else owningEntity.sprite.Play("spawn"); 
        if (owningEntity.stunned) owningEntity.sprite.Play("stun");
        owningEntity.Velocity = Vector3.Zero;
        if (owningEntity.dead) EmitSignal(SignalName.transition, "die");
        if ((owningEntity.awake && (Global.Singleton.player.GlobalPosition - owningEntity.GlobalPosition).Length() <= owningEntity.detectionRange && owningEntity.inview) || owningEntity.damaged) EmitSignal(SignalName.transition, "chase");
    }
}
