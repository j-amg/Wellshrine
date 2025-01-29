using Godot;
using System;

public partial class DashState : State
{
    [Export]
    float speed = 5;
    [Export]
    float acceleration = 1f;
    [Export]
    float deceleration = 0.01f;
	[Export]
	float dashVelocity = 20f;
    public override void Update(double delta)
	{
        player.UpdateInput(speed, acceleration, deceleration);
        player.velocity = -player.head.GlobalBasis.Z * dashVelocity;
        player.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	} 
}
