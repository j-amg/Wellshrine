using Godot;
using System;

public partial class DashState : State
{
    public override void Update(double delta)
	{
        owner.UpdateInput(owner.currentSpeed, 1, 0);
        owner.velocity = -new Vector3(owner.head.GlobalBasis.Z.X, Math.Min(owner.head.GlobalBasis.Z.Y, -0.25f), owner.head.GlobalBasis.Z.Z) * owner.dashVelocity;
        owner.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	} 
}
