using Godot;
using System;

public partial class JumpState : State
{
    public override void Enter()
    {
        owner.UpdateInput(owner.currentSpeed, 1, owner.deceleration);
        Vector3 vel = owner.velocity;
        vel.Y += 5;
        owner.velocity = vel;
    }
    public override void Update(double delta)
	{
        owner.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	}
}
