using Godot;
using System;

public partial class SlideState : State
{
    private float speed;

    public override void Enter()
    {
        speed = owner.slideSpeed;
        owner.SetCrouch(true);
    }

    public override void Exit()
    {
        owner.SetCrouch(false);
    }
    public override void Update(double delta)
	{
        
		speed -= .75f;
        owner.UpdateInput(speed, owner.acceleration, owner.deceleration);
        owner.UpdateVelocity();
		if (speed <= owner.crouchSpeed) EmitSignal(SignalName.transition, "crouch");
		if (owner.velocity != Vector3.Zero && Input.IsActionJustReleased("Shift")) EmitSignal(SignalName.transition, "walk");
		if (Input.IsActionJustPressed("Space") && !owner.inputPaused) EmitSignal(SignalName.transition, "dash");
        if (!owner.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
	}
}
