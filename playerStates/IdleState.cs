using Godot;
using System;
public partial class IdleState : State
{
    public override void Update(double delta)
    {
        owningEntity.currentJump = 0;
        owningEntity.currentDash = 0;
        owningEntity.UpdateInput(owningEntity.currentSpeed, owningEntity.acceleration, owningEntity.deceleration);
        owningEntity.UpdateVelocity();
	    if (owningEntity.Velocity.Length() > 0.0 && owningEntity.IsOnFloor()) EmitSignal(SignalName.transition, "walk");
        if (!owningEntity.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
        if (Input.IsActionJustPressed("Shift")) EmitSignal(SignalName.transition, "crouch");
        if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "jump");
    }
}
