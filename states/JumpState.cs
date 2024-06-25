using Godot;
using System;

public partial class JumpState : State
{
    [Export]
    float speed = 5;
    [Export]
    float acceleration = 0.1f;
    [Export]
    float deceleration = 0.25f;
	[Export]
	float jumpVelocity = 4.5f;

    public override void Enter()
    {
        player.velocity.Y += jumpVelocity;
    }
    public override void Update(double delta)
	{
        //player.UpdateGravity(delta);
        player.UpdateInput(speed, acceleration, deceleration);
        player.UpdateVelocity();

		EmitSignal(SignalName.transition, "fallState");
	} 
}
