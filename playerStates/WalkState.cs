using Godot;
using System;

public partial class WalkState : State
{
	public override void Update(double delta)
	{
        owner.currentJump = 0;
        owner.currentDash = 0;
        owner.UpdateInput(owner.currentSpeed, owner.acceleration, owner.deceleration);
        owner.UpdateVelocity();
		if (owner.Velocity.Length() == 0.0) EmitSignal(SignalName.transition, "idle");
        if (Input.IsActionJustPressed("Shift")) EmitSignal(SignalName.transition, "slide");
        if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "jump");
        if(!owner.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
	}
}
