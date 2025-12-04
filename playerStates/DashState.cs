using Godot;
using System;

public partial class DashState : State
{
    public override void Update(double delta)
	{
        owner.currentJump++;
        owner.currentDash++;
        owner.UpdateInput(owner.currentSpeed, .9f, 0);

        float dashAngle = owner.IsOnFloor() ? -0.25f : owner.head.GlobalBasis.Z.Y;
        Vector3 dashDir = -new Vector3(owner.head.GlobalBasis.Z.X, Math.Min(owner.head.GlobalBasis.Z.Y, dashAngle), owner.head.GlobalBasis.Z.Z).Normalized();
        owner.velocity = dashDir * owner.dashVelocity;
        owner.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	} 
}
