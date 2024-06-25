using Godot;
using System;

public partial class FallState : State
{
	[Export]
    float speed = 2;
    [Export]
    float acceleration = 0.01f;
    [Export]
    float deceleration = 0.025f;
	[Signal]
	public delegate void landedEventHandler();
	public override void Update(double delta)
	{
        player.UpdateGravity(delta);
        player.UpdateInput(speed, acceleration, deceleration);
        player.UpdateVelocity();
		if (player.IsOnFloor())
		{
			EmitSignal(SignalName.landed);
			EmitSignal(SignalName.transition, "idleState");
		}

	} 
}
