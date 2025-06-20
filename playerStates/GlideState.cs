using Godot;
using System;

public partial class GlideState : State
{
	private float s;
    public override void Enter()
    {
		owner.handSprite.Play("glide");
		s = owner.hvel.Length() > owner.fallSpeed ? owner.hvel.Length() : owner.fallSpeed;
    }

    public override void Exit()
    {
		owner.handSprite.Play(Global.Singleton.currentIdle);
    }
    public override void Update(double delta)
	{
        owner.UpdateInput(s, owner.airAcceleration, owner.airDeceleration);
		Vector3 vel = owner.velocity;
		vel.Y = vel.Y > 0 ? Mathf.MoveToward(vel.Y, 0, 0.1f) : Mathf.MoveToward(vel.Y, -3, 0.01f);
		owner.velocity = vel;
        owner.UpdateVelocity();
		if (Input.IsActionJustPressed("Space") && !Input.IsActionPressed("Shift") && !owner.inputPaused && owner.currentJump < owner.maxJumpCount) EmitSignal(SignalName.transition, "jump");
		if (Input.IsActionJustPressed("Space") && !Input.IsActionPressed("Shift") && !owner.inputPaused && owner.nearWall) EmitSignal(SignalName.transition, "walljump");
		if (Input.IsActionJustPressed("Space") && Input.IsActionPressed("Shift") && !owner.inputPaused && owner.currentJump < owner.maxJumpCount && owner.currentDash < owner.maxDashCount) EmitSignal(SignalName.transition, "dash");
		if (Input.IsActionJustReleased("RightMouse")) EmitSignal(SignalName.transition, "fall");
		if (owner.IsOnFloor())
		{
			if (Input.IsActionPressed("Shift"))
			{
				EmitSignal(SignalName.transition, "slide");
			}
			else EmitSignal(SignalName.transition, "idle");
		}
	} 
}
