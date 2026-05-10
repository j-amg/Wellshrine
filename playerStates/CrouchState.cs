using Godot;
using System;

public partial class CrouchState : State
{
    public override void Enter()
    {
        owningEntity.SetCrouch(true);
    }

    public override void Exit()
    {
        owningEntity.SetCrouch(false);
    }
    public override void Update(double delta)
	{
        if (Input.IsActionPressed("Shift"))
        {
            owningEntity.UpdateInput(owningEntity.crouchSpeed, 1f, 1);
        } else owningEntity.UpdateInput(owningEntity.crouchSpeed, owningEntity.acceleration, owningEntity.deceleration);
        
        owningEntity.UpdateVelocity();
		if (Input.IsActionJustReleased("Shift")) EmitSignal(SignalName.transition, "walk");
		if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "dash");
        if (!owningEntity.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
	}
}
