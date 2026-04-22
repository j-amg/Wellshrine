using Godot;
using System;

public partial class JumpState : State
{
    public override void Enter()
    {
        owningEntity.currentJump++;
        owningEntity.UpdateInput(owningEntity.currentSpeed, 1f, owningEntity.deceleration);
        Vector3 vel = owningEntity.velocity;
        vel.Y = 5;
        owningEntity.velocity = vel;
        owningEntity.JumpAnim();
    }
    public override void Update(double delta)
	{
        owningEntity.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	}
}
