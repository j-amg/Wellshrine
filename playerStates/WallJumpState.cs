using Godot;
using System;

public partial class WallJumpState : State
{
	public override void Enter()
	{
		owner.JumpAnim();
		owner.WallJumpTimer();
		owner.currentJump = 1;
        owner.currentDash = 0;
		owner.UpdateInput(owner.currentSpeed, 1, owner.deceleration);
		Vector3 vel;
		float jumpVel = owner.currentSpeed + owner.wallJumpVelocity;
		if (owner.direction == Vector3.Zero)
		{
			vel = -new Vector3(owner.head.GlobalBasis.Z.X, 0, owner.head.GlobalBasis.Z.Z) * jumpVel;

		}
		else vel = new Vector3(owner.direction.X, 0, owner.direction.Z) * jumpVel;
        vel.Y = 5;
		owner.velocity = vel;
	}
	public override void Update(double delta)
	{
		owner.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	}
}
