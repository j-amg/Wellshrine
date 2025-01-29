using Godot;
using System;

public partial class JumpState : State
{
    [Export]
    float speed = 5;
    [Export]
    float acceleration = 1f;
    [Export]
    float deceleration = 0.25f;
	[Export]
	float jumpVelocity = 5f;
    public override void Enter()
    {
        player.UpdateInput(speed, acceleration, deceleration);
        player.velocity.Y += jumpVelocity;
        base.Enter();
    }
    public override void Update(double delta)
	{
        player.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	} 
}
