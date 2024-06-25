using Godot;
using System;

public partial class SprintState : State
{
	[Export]
    float speed = 7;
    [Export]
    float acceleration = 0.1f;
    [Export]
    float deceleration = 0.25f;
	public override void Update(double delta)
	{
		player.UpdateGravity(delta);
        player.UpdateInput(speed, acceleration, deceleration);
        player.UpdateVelocity();
		if (player.Velocity.Length() == 0.0) EmitSignal(SignalName.transition, "idleState");
		if (Input.IsActionJustReleased("Alt")) EmitSignal(SignalName.transition, "walkState");
		if (Input.IsActionJustReleased("Space")) EmitSignal(SignalName.transition, "jumpState");
	}
}
