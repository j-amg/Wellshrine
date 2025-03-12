using Godot;
using System;

public partial class CrouchState : State
{
    public override void Enter()
    {
        owner.SetCrouch(true);
    }

    public override void Exit()
    {
        owner.SetCrouch(false);
    }
    public override void Update(double delta)
	{
        owner.UpdateInput(owner.crouchSpeed, owner.acceleration, owner.deceleration);
        owner.UpdateVelocity();
		if (Input.IsActionJustReleased("Shift")) EmitSignal(SignalName.transition, "walk");
		if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "dash");
        if (!owner.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
	}
}
