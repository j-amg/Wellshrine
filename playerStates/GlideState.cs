using Godot;
using System;

public partial class GlideState : State
{
	private float s;
	private bool isAimDown = false;
    public override void Enter()
    {
		owningEntity.handSprite.Play("glide");
		s = owningEntity.hvel.Length() > owningEntity.fallSpeed ? owningEntity.hvel.Length() : owningEntity.fallSpeed;
		GD.Print("entered glide");
    }

    public override void Exit()
    {
		owningEntity.handSprite.Play(Global.Singleton.currentIdle);
    }
    public override void Update(double delta)
	{
        owningEntity.UpdateInput(s, owningEntity.airAcceleration, owningEntity.airDeceleration);
		Vector3 vel = owningEntity.velocity;
		//vel.Y = vel.Y > 0 ? Mathf.MoveToward(vel.Y, 0, 0.1f) : Mathf.MoveToward(vel.Y, -3, 0.01f);

        vel.Y = Mathf.MoveToward(vel.Y, -1, .25f);
		// if (vel.Y > 0)
		// {
		// 	vel.Y -= owningEntity.gravity * (float)delta;
		// }
		// else
		// {
		// 	vel.Y -= 6 * (float)delta;
		
        // }
		owningEntity.velocity = vel;
        owningEntity.UpdateVelocity();

		//if (Input.IsActionPressed("RightMouse")) isAimDown = true;
		//if (Input.IsActionJustReleased("RightMouse")) EmitSignal(SignalName.transition, "fall");

		//GD.Print(isAimDown);

		if (Input.IsActionJustPressed("Shift")) EmitSignal(SignalName.transition, "slide");
		if (Input.IsActionJustPressed("Space") && !Input.IsActionPressed("Shift") && owningEntity.currentJump < owningEntity.maxJumpCount) EmitSignal(SignalName.transition, "jump");
		if (Input.IsActionJustPressed("Space") && !Input.IsActionPressed("Shift") && owningEntity.nearWall) EmitSignal(SignalName.transition, "walljump");
		if (Input.IsActionJustPressed("Space") && Input.IsActionPressed("Shift") && owningEntity.currentJump < owningEntity.maxJumpCount && owningEntity.currentDash < owningEntity.maxDashCount) EmitSignal(SignalName.transition, "dash");
		//GD.Print(owningEntity.attackStateMachine.current_state.Name);
		if (!Input.IsActionPressed("RightMouse") && owningEntity.attackStateMachine.current_state.Name != "charge") EmitSignal(SignalName.transition, "fall");



		if (owningEntity.IsOnFloor())
		{
			if (Input.IsActionPressed("Shift"))
			{
				EmitSignal(SignalName.transition, "slide");
			}
			else EmitSignal(SignalName.transition, "idle");
		}
	} 
}
