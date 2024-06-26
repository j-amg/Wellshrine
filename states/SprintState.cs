using Godot;
using System;

public partial class SprintState : State
{
	[Export]
    float speed = 10;
    [Export]
    float acceleration = .25f;
    [Export]
    float deceleration = 0.25f;
	public override void Update(double delta)
	{
		player.UpdateInput(speed, acceleration, deceleration);
        player.UpdateVelocity();
		if (player.Velocity.Length() == 0.0) EmitSignal(SignalName.transition, "idleState");
		if (Input.IsActionJustReleased("Shift")) EmitSignal(SignalName.transition, "walkState");
		if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "dashState");
		if(!player.IsOnFloor()) EmitSignal(SignalName.transition, "fallState");
	}
}
