using Godot;
using System;

public partial class DashState : State
{
    public override void Update(double delta)
	{
        owningEntity.currentJump++;
        owningEntity.currentDash++;
        owningEntity.UpdateInput(owningEntity.currentSpeed, .9f, 0);

        float dashAngle = owningEntity.IsOnFloor() ? -0.25f : owningEntity.head.GlobalBasis.Z.Y;
        Vector3 dashDir = -new Vector3(owningEntity.head.GlobalBasis.Z.X, Math.Min(owningEntity.head.GlobalBasis.Z.Y, dashAngle), owningEntity.head.GlobalBasis.Z.Z).Normalized();
        owningEntity.velocity = dashDir * owningEntity.dashVelocity;
        owningEntity.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	} 
}
