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
	float dashVelocity = 15f;
    public override void Update(double delta)
	{
        player.UpdateInput(speed, acceleration, deceleration);
        //player.velocity = -player.head.GlobalBasis.Z * dashVelocity;

        player.velocity = -new Vector3(player.head.GlobalBasis.Z.X, Math.Min(player.head.GlobalBasis.Z.Y, -0.25f), player.head.GlobalBasis.Z.Z) * dashVelocity;
        // GD.Print(Math.Min(player.head.GlobalBasis.Z.Y, -0.25f));
        // GD.Print(player.GlobalBasis.Z.Y);
        // GD.Print(player.GlobalBasis.Z);
        player.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	} 
}
