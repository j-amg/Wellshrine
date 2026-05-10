using Godot;
using System;

public partial class WallJumpState : State
{
	public override void Enter()
	{
		owningEntity.JumpAnim();
		owningEntity.WallJumpTimer();
		owningEntity.currentJump = 1;
        owningEntity.currentDash = 0;
		owningEntity.UpdateInput(owningEntity.currentSpeed, 1, owningEntity.deceleration);
		Vector3 vel;
		float jumpVel = owningEntity.currentSpeed + owningEntity.wallJumpVelocity;
		if (owningEntity.direction == Vector3.Zero)
		{
			vel = -new Vector3(owningEntity.head.GlobalBasis.Z.X, 0, owningEntity.head.GlobalBasis.Z.Z) * jumpVel;

		}
		else vel = new Vector3(owningEntity.direction.X, 0, owningEntity.direction.Z) * jumpVel;
        vel.Y = 5;
		owningEntity.velocity = vel;
	}
	public override void Update(double delta)
	{
		owningEntity.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	}
}
