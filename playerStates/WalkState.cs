using Godot;
using System;

public partial class WalkState : State
{
	public override void Update(double delta)
	{
        owner.UpdateInput(owner.currentSpeed + Global.Singleton.playerMoveSpeedBuff, owner.acceleration, owner.deceleration);
        owner.UpdateVelocity();
		if (owner.Velocity.Length() == 0.0) EmitSignal(SignalName.transition, "idle");
        if (Input.IsActionPressed("Shift") && !owner.inputPaused) EmitSignal(SignalName.transition, "slide");
        if (Input.IsActionJustPressed("Space") && !owner.inputPaused) EmitSignal(SignalName.transition, "jump");
        if(!owner.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
	}
}
