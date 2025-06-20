using Godot;
using System;
public partial class IdleState : State
{
    public override void Update(double delta)
    {
        owner.currentJump = 0;
        owner.currentDash = 0;
        owner.UpdateInput(owner.currentSpeed, owner.acceleration, owner.deceleration);
        owner.UpdateVelocity();
	    if (owner.Velocity.Length() > 0.0 && owner.IsOnFloor()) EmitSignal(SignalName.transition, "walk");
        if (!owner.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
        if (Input.IsActionPressed("Shift") && !owner.inputPaused) EmitSignal(SignalName.transition, "crouch");
        if (Input.IsActionJustPressed("Space") && !owner.inputPaused) EmitSignal(SignalName.transition, "jump");
    }
}
