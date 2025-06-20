using Godot;
using System;

public partial class DashState : State
{
    public override void Update(double delta)
	{
        owner.currentJump++;
        owner.currentDash++;
        owner.UpdateInput(owner.currentSpeed, 1, 0);
        if (owner.IsOnFloor())
        {
            owner.velocity = -new Vector3(owner.head.GlobalBasis.Z.X, Math.Min(owner.head.GlobalBasis.Z.Y, -0.25f), owner.head.GlobalBasis.Z.Z) * owner.dashVelocity;
        } else owner.velocity = -new Vector3(owner.head.GlobalBasis.Z.X, owner.head.GlobalBasis.Z.Y, owner.head.GlobalBasis.Z.Z) * owner.dashVelocity;
        
        
        owner.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	} 
}
