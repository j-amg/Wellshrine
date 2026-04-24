using Godot;
using System;

public partial class FallState : State
{
	private float s;

    public override void Enter()
    {	//GD.Print("entered fall state");
		s = owningEntity.hvel.Length() > owningEntity.fallSpeed ? owningEntity.hvel.Length() : owningEntity.fallSpeed;
    }
    public override void Update(double delta)
	{
        owningEntity.UpdateInput(s, .2f, owningEntity.airDeceleration);
		Vector3 vel = owningEntity.velocity;
		vel.Y -= owningEntity.gravity * (float)delta;
		owningEntity.velocity = vel;
        owningEntity.UpdateVelocity();
		if (Input.IsActionJustPressed("RightMouse")) EmitSignal(SignalName.transition, "glide");
		if (Input.IsActionJustPressed("Shift")) EmitSignal(SignalName.transition, "slide");
		if (Input.IsActionJustPressed("Space") && !Input.IsActionPressed("Shift") && owningEntity.currentJump < owningEntity.maxJumpCount) EmitSignal(SignalName.transition, "jump");
		if (Input.IsActionJustPressed("Space") && !Input.IsActionPressed("Shift") && owningEntity.nearWall && owningEntity.canWallJump) EmitSignal(SignalName.transition, "walljump");
		if (Input.IsActionJustPressed("Space") && Input.IsActionPressed("Shift") && owningEntity.currentJump < owningEntity.maxJumpCount && owningEntity.currentDash < owningEntity.maxDashCount) EmitSignal(SignalName.transition, "dash");
		if (owningEntity.attackStateMachine.current_state.Name == "charge")
		{
			GD.Print("Entered charge midarir");
			EmitSignal(SignalName.transition, "glide");
		} 
		

		if (owningEntity.IsOnFloor())
		{
			if (Input.IsActionPressed("Shift"))
			{
				EmitSignal(SignalName.transition, "crouch");
			}
			else EmitSignal(SignalName.transition, "idle");
		}
	} 
}
