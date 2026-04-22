using Godot;
using System;

public partial class WalkState : State
{
	public override void Update(double delta)
	{
        owningEntity.currentJump = 0;
        owningEntity.currentDash = 0;
        owningEntity.UpdateInput(owningEntity.currentSpeed, owningEntity.acceleration, owningEntity.deceleration);
        owningEntity.UpdateVelocity();
		if (owningEntity.Velocity.Length() == 0.0) EmitSignal(SignalName.transition, "idle");
        if (Input.IsActionJustPressed("Shift")) EmitSignal(SignalName.transition, "slide");
        if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "jump");
        if(!owningEntity.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
	}
}
