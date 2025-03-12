using Godot;
using System;

public partial class FallState : State
{
	private float s;

    public override void Enter()
    {
		s = owner.hvel.Length() > owner.fallSpeed ? owner.hvel.Length() : owner.fallSpeed;
    }
    public override void Update(double delta)
	{
        owner.UpdateInput(s, owner.airAcceleration, owner.airDeceleration);
		Vector3 vel = owner.velocity;
		vel.Y -= owner.gravity * (float)delta;
		owner.velocity = vel;
        owner.UpdateVelocity();
		if (Input.IsActionPressed("RightMouse") && !owner.inputPaused) EmitSignal(SignalName.transition, "glide");
		
		if (owner.IsOnFloor())
		{
			if (Input.IsActionPressed("Shift") && !owner.inputPaused)
			{
				EmitSignal(SignalName.transition, "slide");
			}
			else EmitSignal(SignalName.transition, "idle");
		}
	} 
}
