using Godot;
using System;

public partial class EnemyIdle : State
{
    public override void Update(double delta)
    {
        if (owner.awake) owner.sprite.Play("idle"); else owner.sprite.Play("spawn"); 
        if (owner.stunned) owner.sprite.Play("stun");
        owner.Velocity = Vector3.Zero;
        if (owner.dead) EmitSignal(SignalName.transition, "die");
        if ((owner.awake && (Global.Singleton.player.GlobalPosition - owner.GlobalPosition).Length() <= owner.detectionRange && owner.inview) || owner.damaged) EmitSignal(SignalName.transition, "chase");
    }
}
