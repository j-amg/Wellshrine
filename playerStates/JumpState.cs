using Godot;
using System;

public partial class JumpState : State
{
    public override void Enter()
    {
        owner.currentJump++;
        owner.UpdateInput(owner.currentSpeed, 7.5f, owner.deceleration);
        Vector3 vel = owner.velocity;
        vel.Y = 5;
        owner.velocity = vel;
    }
    public override void Update(double delta)
	{
        owner.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	}
}
